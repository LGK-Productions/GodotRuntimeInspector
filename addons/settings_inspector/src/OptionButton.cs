using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public partial class OptionButton : Control
{
    private object[]? _currentOptions;
    public Action<int>? IndexSelected;

    public Action<object>? ItemSelected;
    [Export] private Godot.OptionButton optionButton;

    public int SelectedIndex => optionButton.Selected;
    public object? Selected => _currentOptions?[SelectedIndex];

    public override void _EnterTree()
    {
        base._EnterTree();
        optionButton.ItemSelected += OnItemSelected;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        optionButton.ItemSelected -= OnItemSelected;
    }

    public void SetOptions(IEnumerable<object> options)
    {
        optionButton.GetPopup().Clear();
        _currentOptions = options.ToArray();
        foreach (var option in _currentOptions) optionButton.GetPopup().AddItem(option.ToString());
    }

    public void SetSelectedIndex(int index)
    {
        optionButton.Selected = index;
    }

    private void OnItemSelected(long index)
    {
        var ind = (int)index;
        if (_currentOptions == null || ind >= _currentOptions.Length) return;
        IndexSelected?.Invoke(ind);
        ItemSelected?.Invoke(_currentOptions[ind]);
    }
}