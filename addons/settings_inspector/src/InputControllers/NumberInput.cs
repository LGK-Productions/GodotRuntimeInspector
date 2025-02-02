using System;
using System.Numerics;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class NumberInput<T> : Control, IMemberInput where T : struct, INumber<T>
{
	[Export] private SpinBox _spinBox;

    protected virtual double StepSize { get; } = 1;

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
		_spinBox.Value = (double)Convert.ChangeType(value, typeof(double));
	}

	public object GetValue()
	{
		return Convert.ChangeType(_spinBox.Value, typeof(T));
	}

	public void SetEditable(bool editable)
	{
		_spinBox.Editable = editable;
	}

	private void OnNumberChanged(double value)
	{
		OnValueChanged?.Invoke(value);
	}

	public void OnSetElement(InspectorElement element)
	{
		if (element.MemberInfo.MaxValue != null)
			_spinBox.MaxValue = (double)Convert.ChangeType(element.MemberInfo.MaxValue, typeof(double));
		else
			_spinBox.AllowGreater = true;
		if (element.MemberInfo.MinValue != null)
			_spinBox.MinValue = (double)Convert.ChangeType(element.MemberInfo.MinValue, typeof(double));
		else
			_spinBox.AllowLesser = true;
        _spinBox.Step = StepSize;
    }

	public event Action<object>? OnValueChanged;
}
