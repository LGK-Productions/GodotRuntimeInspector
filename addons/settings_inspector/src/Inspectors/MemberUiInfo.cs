using System;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public record MemberUiInfo(
    bool Scrollable = false,
    bool AllowTabs = false,
    bool IsLabelHidden = false,
    bool IsBackgroundHidden = false,
    bool IsExpanded = false,
    bool HideExpanded = false,
    Type? parentType = null)
{
    public static readonly MemberUiInfo Default = new MemberUiInfo(Scrollable: false, AllowTabs: false, IsLabelHidden: false, IsBackgroundHidden: false, IsExpanded: false, HideExpanded: false, parentType: null);
}