using System;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.InspectorCollections;

namespace SettingInspector.addons.settings_inspector.src.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class GroupStyleAttribute(GroupStyleMode styleMode) : InspectorAttribute
{
    public const string MetadataKey = "GroupStyleMode";

    public GroupStyleMode StyleMode { get; } = styleMode;

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.CustomMetaData.Add(MetadataKey, StyleMode);
    }
}