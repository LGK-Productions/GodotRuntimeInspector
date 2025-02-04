namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public readonly record struct MemberUiInfo (
    bool Scrollable = false, 
    bool AllowTabs = false, 
    bool IsLabelHidden = false, 
    bool IsBackgroundHidden = false);