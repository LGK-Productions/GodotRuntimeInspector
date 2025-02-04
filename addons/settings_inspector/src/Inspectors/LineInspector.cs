using System;
using System.ComponentModel;
using System.Reflection;
using Godot;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class LineInspector : MemberInspector
{
	[Export] private LineEdit _lineEdit;

	public override void _EnterTree()
	{
		base._EnterTree();
		_lineEdit.TextChanged += OnTextChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_lineEdit.TextChanged -= OnTextChanged;
	}

	private void OnTextChanged(string newValue)
	{
		OnValueChanged();
	}

	protected override void SetValue(object? value)
	{
		_lineEdit.Text = value?.ToString();
	}

	protected override object? GetValue()
	{
		return _lineEdit.Text;
	}

	public override void SetEditable(bool editable)
	{
		base.SetEditable(editable);
		_lineEdit.Editable = editable;
	}

}
