using System;
using System.Globalization;
using System.Numerics;
using Godot;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.Attributes;
using SettingInspector.addons.settings_inspector.ValueTree;
using Range = Godot.Range;

namespace SettingInspector.addons.settings_inspector.Inspectors;

public partial class NumberInspector<T> : MemberInspector where T : struct, INumber<T>
{
    private double _internalValue;

    private Range? _range;
    [Export] private Slider? _slider;
    [Export] private SpinBox? _spinBox;
    [Export] private Label? _valueLabel;
    protected virtual double StepSize { get; set; } = 1;

    protected override void OnInitialize()
    {
        _spinBox!.ValueChanged += OnNumberChanged;
        _slider!.ValueChanged += OnNumberChanged;
        _spinBox.GetLineEdit().TextChanged += OnTextChanged;
    }

    private void OnTextChanged(string newtext)
    {
        if (!double.TryParse(newtext, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) return;
        if (value % StepSize <= 0.000001 || value % StepSize >= StepSize - 0.000001) _internalValue = value;
    }

    protected override void OnRemove()
    {
        _spinBox!.ValueChanged -= OnNumberChanged;
        _slider!.ValueChanged -= OnNumberChanged;
    }

    protected override void SetValueInternal(object value)
    {
        _range ??= _slider;
        var val = (double)Convert.ChangeType(value, typeof(double));
        _range!.SetValue(val);
        _internalValue = val;
        SetValueLabel(val);
    }

    protected override object? GetValue()
    {
        return Convert.ChangeType(_internalValue, typeof(T));
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        _spinBox!.Editable = editable;
        _slider!.Editable = editable;
    }

    private void OnNumberChanged(double value)
    {
        _internalValue = value;
        SetValueLabel(_internalValue);
        OnValueChanged(new ValueChangeTree(this, value));
    }

    private void SetValueLabel(double value)
    {
        value *= 100;
        var valInt = (int)Math.Round(value);
        var valueText = (valInt / 100f).ToString(CultureInfo.InvariantCulture);
        _valueLabel!.Text = valueText;
    }

    protected override void OnSetMetaData(MetaDataMember member)
    {
        base.OnSetMetaData(member);
        _range = member.TryGetMetaData(SliderAttribute.MetadataKey, out _)
            ? _slider
            : _spinBox;
        if (member.TryGetMetaData(StepSizeAttribute.MetadataKey, out var stepSize))
            StepSize = stepSize;
        if (member.TryGetMetaData(SuffixAttribute.MetadataKey, out var suffix))
            _spinBox!.Suffix = suffix;
        if (member.MaxValue != null)
            _range!.MaxValue = (double)Convert.ChangeType(member.MaxValue, typeof(double));
        else
            _range!.AllowGreater = true;
        if (member.MinValue != null)
            _range.MinValue = (double)Convert.ChangeType(member.MinValue, typeof(double));
        else
            _range.AllowLesser = true;
        _range.Step = StepSize;
        _slider!.Visible = _range == _slider;
        _valueLabel!.Visible = _range == _slider;
        _spinBox!.Visible = _range == _spinBox;
    }
}