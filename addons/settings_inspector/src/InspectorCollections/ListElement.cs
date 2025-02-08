using Godot;
using System;

public partial class ListElement : Control
{
	[Export] public Button _upButton;
	[Export] public Button _downButton;
	[Export] public Button _deleteButton;
	[Export] private Control _inspectorContainer;

	public void SetUi(Control ui)
	{
		if (ui.GetParent() != null)
			ui.Reparent(_inspectorContainer);
		else
			_inspectorContainer.AddChild(ui);
		ui.AddThemeConstantOverride("margin_top", 0);
		ui.AddThemeConstantOverride("margin_left", 0);
		ui.AddThemeConstantOverride("margin_right", 0);
		ui.AddThemeConstantOverride("margin_bottom", 0);
	}

	private void ClearUi()
	{
		foreach (var child in _inspectorContainer.GetChildren())
		{
			child.QueueFree();
		}
	}
}
