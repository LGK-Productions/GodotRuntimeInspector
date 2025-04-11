using System;
using System.Globalization;
using System.Numerics;
using Godot;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.Attributes;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class NumberInspector<T> : MemberInspector where T : struct, INumber<T>
{
    [Export] private SpinBox _spinBox;
    [Export] private Slider _slider;
    [Export] private Label _valueLabel;

    private Godot.Range? _range;
    protected virtual double StepSize { get; } = 1;

    protected override void OnInitialize()
    {
        _spinBox.ValueChanged += OnNumberChanged;
        _slider.ValueChanged += OnNumberChanged;
    }

    protected override void OnRemove()
    {
        _spinBox.ValueChanged -= OnNumberChanged;
        _slider.ValueChanged -= OnNumberChanged;
    }

    protected override void SetValue(object value)
    {
        _range ??= _slider;
        base.SetValue(value);
        var val = (double)Convert.ChangeType(value, typeof(double));
        _range.Value = val;
        SetValueLabel(val);
    }

    protected override object? GetValue()
    {
        return Convert.ChangeType(_range.Value, typeof(T));
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        _spinBox.Editable = editable;
        _slider.Editable = editable;
    }

    private void OnNumberChanged(double value)
    {
        SetValueLabel(value);   
        OnValueChanged();
    }
    
    private static readonly char[] Seperators = ['.', ','];

    private void SetValueLabel(double value)
    {
        var valueText = value.ToString(CultureInfo.InvariantCulture);
        int index = valueText.IndexOfAny(Seperators);
        _valueLabel.Text = index < 0 || index + 3 >= valueText.Length ? valueText : valueText.Substring(0, index + 3);
    }

    protected override void OnSetMetaData(MetaDataMember member)
    {
        _range = member.TryGetMetaData(new MetaDataKey<bool>(SliderAttribute.MetadataKey), out var metaData)
            ? _slider
            : _spinBox;
        base.OnSetMetaData(member);
        if (member.MaxValue != null)
            _range.MaxValue = (double)Convert.ChangeType(member.MaxValue, typeof(double));
        else
            _range.AllowGreater = true;
        if (member.MinValue != null)
            _range.MinValue = (double)Convert.ChangeType(member.MinValue, typeof(double));
        else
            _range.AllowLesser = true;
        _range.Step = StepSize;
        _slider.Visible = _range == _slider;
        _valueLabel.Visible = _range == _slider;
        _spinBox.Visible = _range == _spinBox;
    }
}