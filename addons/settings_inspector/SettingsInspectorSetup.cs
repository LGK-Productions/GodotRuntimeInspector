#if TOOLS
using Godot;

namespace SettingInspector.addons.settings_inspector;

[Tool]
public partial class SettingsInspectorSetup : EditorPlugin
{
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
    }
}
#endif