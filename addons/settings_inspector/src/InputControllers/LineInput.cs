using System;
using System.ComponentModel;
using System.Reflection;
using Godot;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class LineInput : Control, IMemberInput
{
	[Export] private LineEdit _lineEdit;

	public override void _Ready()
	{
		_lineEdit.TextChanged += OnTextChanged;
	}

	public override void _ExitTree()
	{
		_lineEdit.TextChanged -= OnTextChanged;
	}

	private void OnTextChanged(string newValue)
	{
		OnValueChanged?.Invoke(newValue);
	}

	public void SetValue(object value)
	{
		_lineEdit.Text = value?.ToString();
	}

	public object GetValue()
	{
		return _lineEdit.Text;
	}

	public void SetEditable(bool editable)
	{
		_lineEdit.Editable = editable;
	}

	public event Action<object>? OnValueChanged;
}
