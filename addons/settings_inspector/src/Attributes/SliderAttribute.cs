using System;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SliderAttribute : InspectorAttribute
{
    public static readonly MetaDataKey<bool> MetadataKey = new("Slider");

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.SetMetaData(MetadataKey, true);
    }
}