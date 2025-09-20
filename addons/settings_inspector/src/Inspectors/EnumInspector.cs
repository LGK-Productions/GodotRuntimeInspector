using System;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.ValueTree;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class EnumInspector : MemberInspector
{
    private readonly List<string> _enumLabels = new();
    [Export] private Godot.OptionButton _optionButton;

    protected override void OnInitialize()
    {
        _optionButton.ItemSelected += OnItemSelected;
    }

    protected override void OnRemove()
    {
        _optionButton.ItemSelected -= OnItemSelected;
    }

    private void OnItemSelected(long item)
    {
        OnValueChanged(new ValueChangeTree(this, item));
    }

    protected override void SetValue(object value)
    {
        base.SetValue(value);
        if (Enum.IsDefined(value.GetType(), value) == false || ValueType == null) return;
        var name = Enum.GetName(ValueType, value);
        if (name == null) return;
        _optionButton.Selected = _enumLabels.IndexOf(name);
    }

    protected override object? GetValue()
    {
        return ValueType == null ? null : Enum.Parse(ValueType, _enumLabels[_optionButton.Selected]);
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        _optionButton.Disabled = !editable;
    }

    protected override void OnSetMetaData(MetaDataMember member)
    {
        base.OnSetMetaData(member);
        foreach (var label in Enum.GetNames(member.Type))
        {
            _optionButton.AddItem(label);
            _enumLabels.Add(label);
        }
    }
}
