using System;

namespace SettingInspector.addons.settings_inspector.Inspectors;

public record MemberUiInfo(
    bool Scrollable = false,
    bool AllowTabs = false,
    Type? ParentType = null)
{
    public static readonly MemberUiInfo Default = new();
}