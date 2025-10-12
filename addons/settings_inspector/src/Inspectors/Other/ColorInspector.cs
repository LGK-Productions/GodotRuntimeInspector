using Godot;
using SettingInspector.Util;

namespace SettingInspector.Inspectors.Other;

public partial class ColorInspector : MemberInspector
{
    [Export] private ColorPickerButton? _colorPicker;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        _colorPicker!.ColorChanged += OnColorChanged;
    }

    protected override void OnRemove()
    {
        base.OnRemove();
        _colorPicker!.ColorChanged -= OnColorChanged;
    }

    protected override void SetValueInternal(object value)
    {
        if (value is Color color)
            _colorPicker!.Color = color;
    }

    protected override object? GetValue()
    {
        return _colorPicker!.Color;
    }

    private void OnColorChanged(Color color)
    {
        OnValueChanged(new ValueChangeTree(this, color));
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        _colorPicker!.Disabled = !editable;
    }
}