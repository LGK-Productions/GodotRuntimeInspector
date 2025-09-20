using System;
using Godot;
using SettingInspector.addons.settings_inspector.src.ValueTree;

namespace SettingInspector.addons.settings_inspector.src.Inspectors.InspectorCollections;

public partial class ListElement : Control
{
    [Export] private Button _deleteButton;
    [Export] private Button _downButton;

    private MemberInspector? _inspector;
    [Export] private Control _inspectorContainer;
    private ListInspector? _listInspector;
    [Export] private Button _upButton;

    public void SetMemberInspector(MemberInspector inspector, ListInspector listInspector)
    {
        _inspector = inspector;
        _listInspector = listInspector;

        if (_inspector.GetParent() != null)
            _inspector.Reparent(_inspectorContainer);
        else
            _inspectorContainer.AddChild(_inspector);
        _inspector.AddThemeConstantOverride("margin_top", 0);
        _inspector.AddThemeConstantOverride("margin_left", 0);
        _inspector.AddThemeConstantOverride("margin_right", 0);
        _inspector.AddThemeConstantOverride("margin_bottom", 0);

        _inspector.ValueChanged += OnMemberValueChanged;
        _upButton.Pressed += OnUpPressed;
        _downButton.Pressed += OnDownPressed;
        _deleteButton.Pressed += OnDeletePressed;
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

    private void Clear()
    {
        _inspector?.Remove();
        _inspector = null;
        _upButton.Pressed -= OnUpPressed;
        _downButton.Pressed -= OnDownPressed;
        _deleteButton.Pressed -= OnDeletePressed;
    }

    private void OnMemberValueChanged(ValueChangeTree tree)
    {
        ValueChanged?.Invoke(tree);
    }

    public event Action<ValueChangeTree>? ValueChanged;
}
