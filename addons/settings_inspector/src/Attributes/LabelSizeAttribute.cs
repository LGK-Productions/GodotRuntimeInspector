using System;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class LabelSizeAttribute(float sizeMultiplier) : InspectorAttribute
{
    public static readonly MetaDataKey<float> MetadataKey = new("LabelSize");

    public float SizeMultiplier { get; } = sizeMultiplier;

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.SetMetaData(MetadataKey, SizeMultiplier);
    }
}