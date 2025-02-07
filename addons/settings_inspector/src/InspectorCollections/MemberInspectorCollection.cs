using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src.InspectorCollections;

public partial class MemberInspectorCollection : Control, IMemberInspectorCollection
{
	[Export] private Node _memberInspectorParent;
	[Export] private PackedScene _boxGroupScene;
	[Export] private PackedScene _horizontalGroupScene;
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
		string? groupName = element.MemberInfo.GroupName;
		PackedScene groupScene = _boxGroupScene;
		if (groupName == null && element.MemberInfo.CustomMetaData.TryGetValue("HorizontalGroupName", out var value) &&
			value is string hGroupName)
		{
			groupName = hGroupName;
			groupScene = _horizontalGroupScene;
		}

		if (groupName == null)
			_memberInspectorParent.AddChild(memberInspector);
		else
		{
			MemberGroup? group = null;
			if (!_memberGroups.TryGetValue(groupName, out group))
			{
				var memberGroupNode = groupScene.Instantiate();
				_memberInspectorParent.AddChild(memberGroupNode);
				group = (MemberGroup)memberGroupNode;
				group.SetGroup(groupName);
				_memberGroups.Add(groupName, group);
			}

			group.AddMember(memberInspector);
			if (element.MemberInfo.CustomMetaData.TryGetValue("GroupStyleMode", out var val) &&
				val is GroupStyleMode styleMode)
				group.SetStyleMode(styleMode);
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

	private void Clear()
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

	public void Remove()
	{
		Clear();
		QueueFree();
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
