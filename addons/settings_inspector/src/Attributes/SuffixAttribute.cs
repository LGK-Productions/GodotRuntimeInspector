using System;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SuffixAttribute(string suffix) : InspectorAttribute
{
    public static readonly MetaDataKey<string> MetadataKey = new("Suffix");

    public string Suffix { get; } = suffix;

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.SetMetaData(MetadataKey, Suffix);
    }
}