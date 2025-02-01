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
        MethodInfo parseMethod = typeof(T).GetMethod("Parse", new[] { typeof(string) });
        if (parseMethod == null)
        {
            GD.PrintErr($"Parse method from string to {typeof(T).Name} not found");
            return false;
        }
        try
        {
            value = (T)parseMethod.Invoke(null, new object[] { _lineEdit.Text });
        }
        catch
        {
            GD.PrintErr($"Parse from string to {typeof(T).Name} failed");
        }
        return false;
    }
}