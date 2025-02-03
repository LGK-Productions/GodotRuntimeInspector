using System;
using System.ComponentModel;
using Godot;
using Godot.Collections;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.InputControllers;

namespace SettingInspector.addons.settings_inspector.src;

public abstract partial class MemberInspector : Control
{
	[Export] private Label _label;

	protected InspectorElement? InspectorElement;
	
	public override void _ExitTree()
	{
		RemoveMember();
	}
	
	public void SetMember(InspectorElement iElement)
	{
		_label.Text = iElement.MemberInfo.DisplayName;
		_label.TooltipText = iElement.MemberInfo.Description;
		SetEditable(!iElement.MemberInfo.IsReadOnly);
		InspectorElement = iElement;
		
        OnSetMetaData(iElement.MemberInfo);
		
		iElement.ValueChanged += UpdateMemberInputValue;
		SetValue(iElement.Value);
	}
	
	protected abstract object? GetValue();
	protected abstract void SetValue(object? value);
	public abstract void SetEditable(bool editable);
	protected virtual void OnSetMetaData(MetaDataMember member){}
	public event Action ValueChanged;
	protected void OnValueChanged()
	{
		ValueChanged?.Invoke();
	}

	public void RemoveMember()
	{
		if (InspectorElement == null) return;
		InspectorElement.ValueChanged -= UpdateMemberInputValue;
		InspectorElement = null;
	}

	private void UpdateMemberInputValue(object instance, MetaDataMember member, object? value)
	{
		SetValue(value);
	}

	public bool TryRetrieveMember(out object? result)
	{
		result = default;
		if (InspectorElement == null)
		{
			GD.PrintErr("Could not retrieve member, due to no member being set");
			return false;
		}

		try
		{
			result = Convert.ChangeType(GetValue(), InspectorElement.MemberInfo.Type);
			return true;
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}

		return false;
	}
}
