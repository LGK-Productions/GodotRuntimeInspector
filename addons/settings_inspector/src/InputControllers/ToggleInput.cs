using System;
using Godot;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class ToggleInput : Control, IMemberInput
{
	[Export] CheckBox _checkbox;

	public override void _EnterTree()
	{
		_checkbox.Pressed += OnPressed;
	}

	public override void _ExitTree()
	{
		_checkbox.Pressed -= OnPressed;
	}

	public void SetValue(object value)
	{
		_checkbox.SetPressed((bool)value);
	}

	public object GetValue()
	{
		return _checkbox.IsPressed();
	}

	public void SetEditable(bool editable)
	{
		_checkbox.Disabled = !editable;
	}

	private void OnPressed()
	{
		OnValueChanged?.Invoke(GetValue());
	}
	public event Action<object>? OnValueChanged;
}
