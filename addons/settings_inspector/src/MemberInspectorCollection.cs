using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorCollection : Control, IEnumerable<(InspectorElement, MemberInspector)>
{
	[Export] private Node _memberInspectorParent;
	[Export] private PackedScene _memberGroupScene;
	
	private readonly List<(InspectorElement, MemberInspector)> _inspectors = new();

	public void SetMemberInspector(Inspector inspector)
	{
		Clear();
		Dictionary<string, MemberGroup> memberGroups = new();

		foreach (var element in inspector.Elements)
		{
			var scene = MemberInspectorHandler.Instance.GetInputScene(element.MemberInfo.Type);
			var memberInspector = (MemberInspector)scene.Instantiate();

			//Grouping Logic
			if (element.MemberInfo.GroupName == null)
				_memberInspectorParent.AddChild(memberInspector);
			else
			{
				if (memberGroups.TryGetValue(element.MemberInfo.GroupName, out var group))
					group.AddMember(memberInspector);
				else
				{
					var memberGroupNode = _memberGroupScene.Instantiate();
					_memberInspectorParent.AddChild(memberGroupNode);
					var memberGroup = (MemberGroup)memberGroupNode;
					memberGroup.SetGroup(element.MemberInfo.GroupName);
					memberGroups.Add(element.MemberInfo.GroupName, memberGroup);
					memberGroup.AddMember(memberInspector);
				}
			}

			memberInspector.SetMember(element);
			_inspectors.Add((element, memberInspector));
			memberInspector.ValueChanged += OnChildValueChanged;
		}
	}

	public void WriteBack()
	{
		foreach (var (element, inspector) in _inspectors)
		{
			if (inspector.TryRetrieveMember(out var value))
				element.Value = value;
		}
	}

	public void Clear()
	{
		foreach (var (_, inspector) in _inspectors)
		{
			inspector.ValueChanged -= OnChildValueChanged;
			inspector.QueueFree();
		}
		_inspectors.Clear();
	}

	public void SetEditable(bool editable)
	{
		foreach (var (_, inspector) in _inspectors)
		{
			inspector.SetEditable(editable);
		}
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
}
