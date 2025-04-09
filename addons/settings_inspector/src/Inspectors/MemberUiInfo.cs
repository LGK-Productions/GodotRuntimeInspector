using System;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public record MemberUiInfo(
    bool Scrollable = false,
    bool AllowTabs = false,
    Type? parentType = null)
{
    public static readonly MemberUiInfo Default = new();
}