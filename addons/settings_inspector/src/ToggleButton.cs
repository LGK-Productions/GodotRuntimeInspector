using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public partial class ToggleButton : Button
{
	[Export] private Texture2D _onTexture;
	[Export] private Texture2D _offTexture;

	[Export] private bool _startState;
	public override void _Ready()
	{
		Toggled += SetTexture;
		ButtonPressed = _startState;
	}

	private void SetTexture(bool on)
	{
		Icon = on ? _onTexture : _offTexture;
	}
}
