using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src.InspectorCollections;

public partial class MemberInspectorCollection : Control, IMemberInspectorCollection
{
	[Export] private Node _memberInspectorParent;
	[Export] private PackedScene _memberGroupScene;
	[Export] private ScrollContainer _scrollContainer;
	
	private readonly List<(InspectorElement, MemberInspector)> _inspectors = new();
	readonly Dictionary<string, MemberGroup> _memberGroups = new();

	public void SetMemberInspector(Inspector inspector)
	{
		Clear();
		foreach (var element in inspector.Elements)
		{
			AddElement(element);
		}
	}

	public void AddElement(InspectorElement element)
	{
		var scene = MemberInspectorHandler.Instance.GetInputScene(element.MemberInfo.Type);
		var memberInspector = (MemberInspector)scene.Instantiate();

		//Grouping Logic
		if (element.MemberInfo.GroupName == null)
			_memberInspectorParent.AddChild(memberInspector);
		else
		{
			if (_memberGroups.TryGetValue(element.MemberInfo.GroupName, out var group))
				group.AddMember(memberInspector);
			else
			{
				var memberGroupNode = _memberGroupScene.Instantiate();
				_memberInspectorParent.AddChild(memberGroupNode);
				var memberGroup = (MemberGroup)memberGroupNode;
				memberGroup.SetGroup(element.MemberInfo.GroupName);
				_memberGroups.Add(element.MemberInfo.GroupName, memberGroup);
				memberGroup.AddMember(memberInspector);
			}
		}

		memberInspector.SetMember(element);
		_inspectors.Add((element, memberInspector));
		memberInspector.ValueChanged += OnChildValueChanged;
	}

	public void WriteBack()
	{
		foreach (var (element, inspector) in _inspectors)
		{
			if (element.MemberInfo.IsReadOnly) continue;
			if (inspector.TryRetrieveMember(out var value))
				element.Value = value;
		}
	}

	public void Clear()
	{
		foreach (var (_, inspector) in _inspectors)
		{
			inspector.ValueChanged -= OnChildValueChanged;
			inspector.Remove();
		}
		_inspectors.Clear();
		foreach (var (_, group) in _memberGroups)
		{
			group.QueueFree();
		}
		_memberGroups.Clear();
	}

	public void SetEditable(bool editable)
	{
		
	}

	private void OnChildValueChanged()
	{
		ValueChanged?.Invoke();
	}

	public IEnumerator<(InspectorElement, MemberInspector)> GetEnumerator()
	{
		return _inspectors.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	
	public event Action ValueChanged;

	public void SetScrollable(bool scrollable)
	{
		if (!scrollable)
		{
			if (_memberInspectorParent.GetParent() is ScrollContainer scroll)
				_memberInspectorParent.Reparent(scroll.GetParent());
		}
		else
		{
			if (_memberInspectorParent.GetParent() is not ScrollContainer)
				_memberInspectorParent.Reparent(_scrollContainer);
		}
	}
}
