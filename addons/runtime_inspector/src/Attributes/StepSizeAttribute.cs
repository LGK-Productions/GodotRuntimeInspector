using System;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace RuntimeInspector.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class StepSizeAttribute(double stepSize) : InspectorAttribute
{
    public static readonly MetaDataKey<double> MetadataKey = new("StepSize");

    public double StepSize { get; } = stepSize;

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.SetMetaData(MetadataKey, StepSize);
    }
}