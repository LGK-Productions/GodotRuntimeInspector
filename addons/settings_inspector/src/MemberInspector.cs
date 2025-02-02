using System;
using System.ComponentModel;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.InputControllers;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspector : Node
{
	[Export] private Label _label;
	[Export] private Control _inputContainer;
	[Export] private PackedScene _lineInputScene;

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
		
		var lineInput = _lineInputScene.Instantiate<LineInput>();
		_inputContainer.AddChild(lineInput);
		lineInput.SetValue(iElement.Value);

		iElement.ValueChanged += UpdateMemberInputValue;
		
		_inspectorMember = iElement;
		_memberInput = lineInput;
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
}
