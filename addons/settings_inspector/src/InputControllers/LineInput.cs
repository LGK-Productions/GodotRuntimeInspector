using System;
using System.ComponentModel;
using System.Reflection;
using Godot;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class LineInput : Control, IMemberInput
{
	[Export] private LineEdit _lineEdit;
	
	public void SetValue(object value)
	{
		_lineEdit.Text = value?.ToString();
	}

	public bool TryGetValue<T>(out T? value)
	{
		value = default;
		try
		{
			TConverter.ChangeType<T>(_lineEdit.Text);
		}
		catch
		{
			GD.PrintErr($"Parse from string to {typeof(T).Name} failed");
		}
		return false;
	}
}
