using System;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class HorizontalGroupAttribute(string groupName) : InspectorAttribute
{
    public const string MetadataKey = "HorizontalGroupName";

    public string HorizontalGroupName { get; } = groupName;

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.CustomMetaData.Add(MetadataKey, HorizontalGroupName);
    }
}