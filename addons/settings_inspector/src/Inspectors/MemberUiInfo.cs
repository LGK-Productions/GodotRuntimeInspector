using System;

namespace SettingInspector.Inspectors;

public record MemberUiInfo(
    bool Scrollable = false,
    bool AllowTabs = false,
    Type? ParentType = null)
{
    public static readonly MemberUiInfo Default = new();
}