using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorButtonWrapper : Control
{
	[Export] private Label _nameLabel;
	[Export] private Button _cancelButton;
	[Export] private Button _confirmButton;
	[Export] private Node _inspectorContainer;
	[Export] private PackedScene _memberGroupScene;

	private TaskCompletionSource? _tcs;
	List<(InspectorElement, MemberInspector)> _inspectors = new();

	public override void _Ready()
	{
		_cancelButton.Pressed += () => _tcs?.SetCanceled();
		_confirmButton.Pressed += () => _tcs?.SetResult();
	}

	public async Task<T> SetInspector<T>(T instance)
	{
		if (_tcs != null)
		{
			GD.PrintErr("Inspector is already open");
			return instance;
		}
		
		_tcs = new TaskCompletionSource();
		_nameLabel.Text = typeof(T).Name;

		//TODO: massive code doubling with class container!!
		var inspector = Inspector.Attach(instance, ClassInspector.TickProvider);
		Dictionary<string, MemberGroup> memberGroups = new();
		
		foreach (var element in inspector.Elements)
		{
			var scene = MemberInspectorHandler.Instance.GetInputScene(element.MemberInfo.Type);
			var memberInspector = (MemberInspector)scene.Instantiate();
			
			//Grouping Logic
			if (element.MemberInfo.GroupName == null)
				_inspectorContainer.AddChild(memberInspector);
			else
			{
				if (memberGroups.TryGetValue(element.MemberInfo.GroupName, out var group))
					group.AddMember(memberInspector);
				else
				{
					var memberGroupNode = _memberGroupScene.Instantiate();
					_inspectorContainer.AddChild(memberGroupNode);
					var memberGroup = (MemberGroup)memberGroupNode;
					memberGroup.SetGroup(element.MemberInfo.GroupName);
					memberGroups.Add(element.MemberInfo.GroupName, memberGroup);
					memberGroup.AddMember(memberInspector);
				}
			}
			
			memberInspector.SetMember(element);
			_inspectors.Add((element, memberInspector));
		}

		try
		{
			await _tcs.Task;
			foreach (var (e, i) in _inspectors)
			{
				if (i.TryRetrieveMember(out var value))
					e.Value = value;
			}
			return instance;
		}
		finally
		{
			_tcs = null;
		}
	}
}
