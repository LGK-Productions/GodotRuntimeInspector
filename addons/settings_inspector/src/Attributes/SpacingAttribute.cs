using System;
using LgkProductions.Inspector;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SpacingAttribute(int top = 0, int bottom = 0, int left = 0, int right = 0) : InspectorAttribute
{
    public static readonly MetaDataKey<string> MetadataKey = new ("Suffix");

    public int Top { get; } = top;
    public int Bottom { get; } = bottom;
    public int Left { get; } = left;
    public int Right { get; } = right;

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.Spacing = new Thickness(Left, Top, Right, Bottom);
    }
}
