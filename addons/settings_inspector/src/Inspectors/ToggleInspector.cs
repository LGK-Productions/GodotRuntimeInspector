using System;
using Godot;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class ToggleInspector : MemberInspector
{
    [Export] CheckBox _checkbox;

    public override void _EnterTree()
    {
        base._EnterTree();
        _checkbox.Pressed += OnPressed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _checkbox.Pressed -= OnPressed;
    }

    protected override void SetValue(object value)
    {
        _checkbox.SetPressed((bool)value);
    }

    protected override object? GetValue()
    {
        return _checkbox.IsPressed();
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        _checkbox.Disabled = !editable;
    }

    private void OnPressed()
    {
        OnValueChanged();
    }
}