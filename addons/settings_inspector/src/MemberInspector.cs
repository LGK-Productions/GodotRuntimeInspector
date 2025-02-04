using System;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.Inspectors;

namespace SettingInspector.addons.settings_inspector.src;

public abstract partial class MemberInspector : Control
{
	[Export] private Label _label;
	[Export] private Control _background;
	[Export] private Control _labelContainer;
	
	protected Type? ValueType;
	
	protected bool Editable = true;
	protected MemberUiInfo MemberUiInfo;
	
	public void SetMember(InspectorElement iElement)
	{
		_label.Text = iElement.MemberInfo.DisplayName;
		_label.TooltipText = iElement.MemberInfo.Description;
		
		SetEditable(!iElement.MemberInfo.IsReadOnly);
		OnSetMetaData(iElement.MemberInfo);
		SetInstance(iElement.Value);

		iElement.ValueChanged += UpdateMemberInputValue;
	}

	/// <summary>
	/// Sets an input value, removing the name label description, etc
	/// </summary>
	/// <param name="value"></param>
	public void SetInstance(object? value, MemberUiInfo memberUiInfo = new())
	{
		ValueType = value?.GetType();
		SetMemberUiInfo(memberUiInfo);
		SetValue(value);
	}

	protected virtual void SetMemberUiInfo(MemberUiInfo memberUiInfo)
	{
		MemberUiInfo = memberUiInfo;
		_labelContainer?.SetVisible(!memberUiInfo.IsLabelHidden);
		_label?.SetVisible(!memberUiInfo.IsLabelHidden);
		_background?.SetVisible(!memberUiInfo.IsBackgroundHidden);
	}
	
	protected abstract object? GetValue();
	protected abstract void SetValue(object? value);

	public virtual void SetEditable(bool editable)
	{
		Editable = editable;
	}
	
	protected virtual void OnSetMetaData(MetaDataMember member){}
	public event Action ValueChanged;
	protected void OnValueChanged()
	{
		ValueChanged?.Invoke();
	}

	private void UpdateMemberInputValue(object instance, MetaDataMember member, object? value)
	{
		SetValue(value);
	}

	public bool TryRetrieveMember(out object? result)
	{
		result = default;
		if (ValueType == null)
		{
			GD.PrintErr("Could not retrieve member, due to no type being set");
			return false;
		}

		try
		{
			result = Convert.ChangeType(GetValue(), ValueType);
			return true;
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}

		return false;
	}
}
