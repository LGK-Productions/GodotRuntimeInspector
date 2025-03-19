using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public partial class ToggleButton : Button
{
	[Export] private Texture2D _onTexture;
	[Export] private Texture2D _offTexture;

	public override void _EnterTree()
	{
		Toggled += SetTexture;
	}

    public override void _ExitTree()
    {
        Toggled -= SetTexture;
    }

	private void SetTexture(bool on)
	{
		Icon = on ? _onTexture : _offTexture;
	}
}
