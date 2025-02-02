using System;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class EnumInput : Control, IMemberInput
{
    [Export] private OptionButton _optionButton;
    
    private List<string> _enumLabels = new();

    private InspectorElement _element;

    public override void _EnterTree()
    {
        _optionButton.ItemSelected += OnItemSelected;
    }

    public override void _ExitTree()
    {
        _optionButton.ItemSelected -= OnItemSelected;
    }

    private void OnItemSelected(long item)
    {
        OnValueChanged?.Invoke(GetValue());
    }

    public void SetValue(object value)
    {
        _optionButton.Selected = _enumLabels.IndexOf(Enum.GetName(_element.MemberInfo.Type, value));
    }

    public object GetValue()
    {
        return Enum.Parse(_element.MemberInfo.Type, _enumLabels[_optionButton.Selected]);
    }

    public void SetEditable(bool editable)
    {
        _optionButton.Disabled = !editable;
    }

    public void OnSetElement(InspectorElement element)
    {
        _element = element;
        foreach (var label in Enum.GetNames(element.MemberInfo.Type))
        {
            _optionButton.AddItem(label);
            _enumLabels.Add(label);
        }
    }

    public event Action<object>? OnValueChanged;
}