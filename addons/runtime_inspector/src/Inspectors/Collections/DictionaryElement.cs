using System;
using Godot;
using RuntimeInspector.Util;

namespace RuntimeInspector.Inspectors.Collections;

public partial class DictionaryElement : Control
{
    [Export] private Button? _deleteButton;
    [Export] private Control? _keyParent;
    [Export] private Control? _valueParent;


    private MemberInspector? _keyInspector;
    private MemberInspector? _valueInspector;

    private DictionaryInspector? _dictionaryInspector;

    public object? GetKey()
    {
        return _keyInspector.TryRetrieveMember(out var key) ? key : null;
    }

    public object? GetValue()
    {
        return _valueInspector.TryRetrieveMember(out var value) ? value : null;
    }

    public void SetMemberInspector(MemberWrapper key, MemberWrapper value, DictionaryInspector dictionaryInspector)
    {
        _keyInspector = key.MemberInspector;
        _valueInspector = value.MemberInspector;

        //Parent key/value Inspectors
        if (key.GetParent() != null)
            key.Reparent(_keyParent);
        else
            _keyParent!.AddChild(key);

        if (value.GetParent() != null)
            value.Reparent(_valueParent);
        else
            _valueParent!.AddChild(value);

        _keyInspector.SetEditable(false);


        key.AddThemeConstantOverride("margin_top", 0);
        key.AddThemeConstantOverride("margin_left", 0);
        key.AddThemeConstantOverride("margin_right", 0);
        key.AddThemeConstantOverride("margin_bottom", 0);

        value.AddThemeConstantOverride("margin_top", 0);
        value.AddThemeConstantOverride("margin_left", 0);
        value.AddThemeConstantOverride("margin_right", 0);
        value.AddThemeConstantOverride("margin_bottom", 0);

        _valueInspector.ValueChanged += OnMemberValueChanged;
        _dictionaryInspector = dictionaryInspector;
        _deleteButton!.Pressed += OnDeletePressed;
    }

    private void OnDeletePressed()
    {
        _dictionaryInspector?.RemoveDictionaryElement(this, removeFromDict: true);
    }

    public void Remove()
    {
        _keyInspector?.Remove();
        _valueInspector?.Remove();
        _deleteButton!.Pressed -= OnDeletePressed;
        QueueFree();
    }

    public void SetEditable(bool editable)
    {
        _valueInspector?.SetEditable(editable);
    }

    private void OnMemberValueChanged(ValueChangeTree tree)
    {
        ValueChanged?.Invoke(tree);
    }

    public event Action<ValueChangeTree>? ValueChanged;
}