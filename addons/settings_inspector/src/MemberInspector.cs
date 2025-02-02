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
		_label.Text = iElement.MemberInfo.Name;
		_label.TooltipText = iElement.MemberInfo.Description;
		
		var node = GetInputScene(iElement.MemberInfo.Type).Instantiate<Control>();
		var memberInput = (IMemberInput)node;
		_inputContainer.AddChild(node);
		memberInput.SetValue(iElement.Value);
		memberInput.SetEditable(!iElement.MemberInfo.IsReadOnly);

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

	private void UpdateMemberInputValue(object instance, InspectorMember member, object? value)
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
			var converter = TypeDescriptor.GetConverter(_inspectorMember.MemberInfo.Type);
			result = converter.ConvertFrom(_memberInput.GetValue());
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

		return _defaultInputScene;
	}
}
