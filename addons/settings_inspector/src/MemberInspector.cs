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

	public Type? ValueType { get; private set; }

	protected bool Editable = true;
	protected MemberUiInfo MemberUiInfo;
	
	private InspectorElement? _element;

	public void SetMember(InspectorElement iElement)
	{
		_element = iElement;
		OnSetMetaData(_element.MemberInfo);
		var value = _element.Value;
		if (value == null && !TryCreateInstance(_element.MemberInfo.Type, out value))
		{
			GD.Print("Value is null, could not create instance.");
			return;
		}

		SetInstance(value!);

		_element.ValueChanged += UpdateMemberInputValue;
	}

	private bool TryCreateInstance(Type type, out object? instance)
	{
		instance = null;
		if (type == typeof(string))
		{
			instance = string.Empty;
			return true;
		}

		try
		{
			instance = Activator.CreateInstance(type);
			return true;
		}
		catch (Exception e)
		{
			return false;
		}
	}

	public void SetInstance(object value) => SetInstance(value, MemberUiInfo.Default);

	public void SetInstance(object value, MemberUiInfo memberUiInfo)
	{
		ValueType = value.GetType();
		if (_element == null)
			_label.Text = ValueType.Name;
		SetMemberUiInfo(memberUiInfo);
		SetValue(value);
	}


	protected virtual void SetMemberUiInfo(MemberUiInfo memberUiInfo)
	{
		MemberUiInfo = memberUiInfo;
		_label?.SetVisible(!memberUiInfo.IsLabelHidden);
		_background?.SetVisible(!memberUiInfo.IsBackgroundHidden);
	}

	protected abstract object? GetValue();

	protected virtual void SetValue(object value)
	{
		Clear();
	}

	public virtual void SetEditable(bool editable)
	{
		Editable = editable;
	}

	protected virtual void OnSetMetaData(MetaDataMember member)
	{
		_label.Text = _element.MemberInfo.DisplayName;
		_label.TooltipText = _element.MemberInfo.Description;
		if (member.CustomMetaData.TryGetValue("LabelSize", out var value) && value is float labelSizeMultiplier)
			_label.SizeFlagsStretchRatio = labelSizeMultiplier;

		SetEditable(!_element.MemberInfo.IsReadOnly);
	}

	protected virtual void Clear()
	{
	}

	public void Remove()
	{
		Clear();
		if (_element != null)
		{
			_element.ValueChanged -= UpdateMemberInputValue;
			_element = null;
		}
		QueueFree();
	}

	public event Action ValueChanged;

	protected void OnValueChanged()
	{
		ValueChanged?.Invoke();
	}

	private void UpdateMemberInputValue(object instance, MetaDataMember member, object? value)
	{
		if (value == null) return;
		SetValue(value);
	}

	public bool TryRetrieveMember(out object? result)
	{
		result = null;
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
