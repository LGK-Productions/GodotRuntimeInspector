using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector;
using SettingInspector.addons.settings_inspector.src.Inspectors;

namespace SettingInspector.addons.settings_inspector.src.InspectorCollections;

public partial class MemberInspectorCollection : Control, IMemberInspectorCollection
{
	[Export] private Node _memberInspectorParent;
	[Export] private PackedScene _boxGroupScene;
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
		var memberWrapper = MemberInspectorHandler.Instance?.MemberWrapperScene?.Instantiate<MemberWrapper>();
		memberWrapper.SetMemberType(element.MemberInfo.Type);
		
		//Grouping Logic
		GroupLayout? groupLayout = element.MemberInfo.Group;
		PackedScene groupScene = _boxGroupScene;

		if (groupLayout == null)
			_memberInspectorParent.AddChild(memberWrapper);
		else
		{
			if (!_memberGroups.TryGetValue(groupLayout.Title, out var group))
			{
				var memberGroupNode = groupScene.Instantiate();
				_memberInspectorParent.AddChild(memberGroupNode);
				group = (MemberGroup)memberGroupNode;
				group.SetGroup(groupLayout);
				_memberGroups.Add(groupLayout.Title, group);
			}

			group.AddMember(memberWrapper);
		}

		memberWrapper.MemberInspector.SetMember(element);
		_inspectors.Add((element, memberWrapper.MemberInspector));
		memberWrapper.MemberInspector.ValueChanged += OnChildValueChanged;
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
		var parent = _memberInspectorParent.GetParent();
		_memberInspectorParent.Owner = null;
		if (!scrollable)
		{
			if (parent is not ScrollContainer) return;
			parent.RemoveChild(_memberInspectorParent);
			parent.GetParent().AddChild(_memberInspectorParent);
		}
		else
		{
			if (parent is ScrollContainer) return;
			parent.RemoveChild(_memberInspectorParent);
			_scrollContainer.AddChild(_memberInspectorParent);
		}

		_memberInspectorParent.Owner = _memberInspectorParent.GetParent();
	}
}
