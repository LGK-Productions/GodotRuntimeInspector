using System;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class IntInput : Control, IMemberInput
{
	[Export] private SpinBox _spinBox;

	public override void _EnterTree()
	{
		_spinBox.ValueChanged += OnNumberChanged;
	}

	public override void _ExitTree()
	{
		_spinBox.ValueChanged -= OnNumberChanged;
	}

	public void SetValue(object value)
	{
		_spinBox.Value = (int) value;
	}

	public object GetValue()
	{
		return (int)_spinBox.Value;
	}

	public void SetEditable(bool editable)
	{
		_spinBox.Editable = editable;
	}

	private void OnNumberChanged(double value)
	{
		OnValueChanged?.Invoke((int)value);
	}

	public void OnSetElement(InspectorElement element)
	{
		if (element.MemberInfo.MaxValue != null)
			_spinBox.MaxValue = (int)element.MemberInfo.MaxValue;
		else
			_spinBox.AllowGreater = true;
		if (element.MemberInfo.MinValue != null)
			_spinBox.MinValue = (int)element.MemberInfo.MinValue;
		else
			_spinBox.AllowLesser = true;
	}

	public event Action<object>? OnValueChanged;
}
