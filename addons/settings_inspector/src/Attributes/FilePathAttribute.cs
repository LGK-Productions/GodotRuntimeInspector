using System;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class FilePathAttribute : InspectorAttribute
{
    public const string MetadataKey = "FilePath";

    public bool FilePath { get; } = true;

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.CustomMetaData.Add(MetadataKey, FilePath);
    }
}