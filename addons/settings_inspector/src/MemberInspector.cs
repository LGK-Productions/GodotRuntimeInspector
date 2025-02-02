using System;
using System.ComponentModel;
using Godot;
using Godot.Collections;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.InputControllers;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspector : Node
{
	[Export] private Label _label;
	[Export] private Control _inputContainer;
	[Export] private PackedScene _defaultInputScene;

	[Export] private Godot.Collections.Array<PackedScene> _inputScenes;

	private InspectorElement? _inspectorMember;
	private IMemberInput? _memberInput;

	
	public override void _ExitTree()
	{
		RemoveMember();
	}
	
	public void SetMember(InspectorElement iElement)
	{
		_label.Text = iElement.MemberInfo.DisplayName;
		_label.TooltipText = iElement.MemberInfo.Description;
		
		var node = GetInputScene(iElement.MemberInfo.Type).Instantiate<Control>();
		var memberInput = (IMemberInput)node;
		_inputContainer.AddChild(node);
		memberInput.SetElement(iElement);
		iElement.ValueChanged += UpdateMemberInputValue;
		
		_inspectorMember = iElement;
		_memberInput = memberInput;
	}

	public void RemoveMember()
	{
		if (_inspectorMember == null) return;
		_inspectorMember.ValueChanged -= UpdateMemberInputValue;
		_inspectorMember = null;
		_memberInput = null;
	}

	private void UpdateMemberInputValue(object instance, MetaDataMember member, object? value)
	{
		_memberInput.SetValue(value);
	}

	public bool TryRetrieveMember(out object? result)
	{
		result = default;
		if (_inspectorMember == null || _memberInput == null)
		{
			GD.PrintErr("Could not retrieve member, due to no member being set");
			return false;
		}

		try
		{
			result = Convert.ChangeType(_memberInput.GetValue(), _inspectorMember.MemberInfo.Type);
			return true;
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}

		return false;
	}

	private PackedScene GetInputScene(Type inputType)
	{
		if (inputType == typeof(bool))
		{
			if (_inputScenes.Count > 0)
				return _inputScenes[0];
		}
		if (inputType == typeof(int))
		{
			if (_inputScenes.Count > 1)
				return _inputScenes[1];
		}
		if (inputType == typeof(float))
		{
			if (_inputScenes.Count > 2)
				return _inputScenes[2];
		}
		if (inputType == typeof(double))
		{
			if (_inputScenes.Count > 3)
				return _inputScenes[3];
		}

		if (inputType.IsEnum)
		{
			if (_inputScenes.Count > 4)
				return _inputScenes[4];
		}

		return _defaultInputScene;
	}
}
