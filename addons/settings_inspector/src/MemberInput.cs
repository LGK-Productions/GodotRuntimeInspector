using System;
using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public interface IMemberInput
{
	public void SetValue(object value);
	
	public object GetValue();

	public void SetEditable(bool editable);

	public event Action<object>? OnValueChanged;
}
