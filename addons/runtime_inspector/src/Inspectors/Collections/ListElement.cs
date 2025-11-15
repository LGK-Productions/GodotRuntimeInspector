using System;
using Godot;
using RuntimeInspector.Util;

namespace RuntimeInspector.Inspectors.Collections;

public partial class ListElement : Control
{
    [Export] private Button? _deleteButton;
    [Export] private Button? _downButton;

    private MemberInspector? _inspector;
    [Export] private Control? _inspectorContainer;
    private ListInspector? _listInspector;
    [Export] private Button? _upButton;

    public void SetMemberInspector(MemberInspector inspector, ListInspector? listInspector = null)
    {
        _inspector = inspector;

        if (_inspector.GetParent() != null)
            _inspector.Reparent(_inspectorContainer);
        else
            _inspectorContainer!.AddChild(_inspector);
        _inspector.AddThemeConstantOverride("margin_top", 0);
        _inspector.AddThemeConstantOverride("margin_left", 0);
        _inspector.AddThemeConstantOverride("margin_right", 0);
        _inspector.AddThemeConstantOverride("margin_bottom", 0);

        _inspector.ValueChanged += OnMemberValueChanged;

        if (listInspector != null)
        {
            _listInspector = listInspector;
            _upButton!.Pressed += OnUpPressed;
            _downButton!.Pressed += OnDownPressed;
            _deleteButton!.Pressed += OnDeletePressed;
            SetButtonsVisible(true);
        }
        else
        {
            SetButtonsVisible(false);
        }
    }

    private void OnUpPressed()
    {
        _listInspector?.MoveElement(this, false);
    }

    private void OnDownPressed()
    {
        _listInspector?.MoveElement(this, true);
    }

    private void OnDeletePressed()
    {
        _listInspector?.RemoveListElement(this);
    }

    public bool TryRetrieveMember(out object? value)
    {
        value = null;
        return _inspector != null && _inspector.TryRetrieveMember(out value);
    }

    public void Remove()
    {
        Clear();
        QueueFree();
    }

    public void SetEditable(bool editable)
    {
        _inspector?.SetEditable(editable);
    }

    private void SetButtonsVisible(bool visible)
    {
        _deleteButton?.SetVisible(visible);
        _downButton?.SetVisible(visible);
        _upButton?.SetVisible(visible);
        (_upButton?.GetParent() as Control)?.SetVisible(visible);
    }

    private void Clear()
    {
        _inspector?.Remove();
        _inspector = null;

        if (_listInspector != null)
        {
            _upButton!.Pressed -= OnUpPressed;
            _downButton!.Pressed -= OnDownPressed;
            _deleteButton!.Pressed -= OnDeletePressed;
        }

        _listInspector = null;
    }

    private void OnMemberValueChanged(ValueChangeTree tree)
    {
        ValueChanged?.Invoke(tree);
    }

    public event Action<ValueChangeTree>? ValueChanged;
}