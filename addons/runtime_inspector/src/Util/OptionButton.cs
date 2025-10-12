using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RuntimeInspector.Util;

public partial class OptionButton : Control
{
    private object[]? _currentOptions;
    [Export] private Godot.OptionButton? _optionButton;
    public Action<int>? IndexSelected;

    public Action<object>? ItemSelected;

    public int SelectedIndex => _optionButton!.Selected;
    public object? Selected => _currentOptions?[SelectedIndex];

    public override void _EnterTree()
    {
        base._EnterTree();
        _optionButton!.ItemSelected += OnItemSelected;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _optionButton!.ItemSelected -= OnItemSelected;
    }

    public void SetOptions(IEnumerable<object> options)
    {
        _optionButton!.GetPopup().Clear();
        _currentOptions = options.ToArray();
        foreach (var option in _currentOptions) _optionButton.GetPopup().AddItem(option.ToString());
    }

    public void SetSelectedIndex(int index)
    {
        _optionButton!.Selected = index;
    }

    private void OnItemSelected(long index)
    {
        var ind = (int)index;
        if (_currentOptions == null || ind >= _currentOptions.Length) return;
        IndexSelected?.Invoke(ind);
        ItemSelected?.Invoke(_currentOptions[ind]);
    }
}