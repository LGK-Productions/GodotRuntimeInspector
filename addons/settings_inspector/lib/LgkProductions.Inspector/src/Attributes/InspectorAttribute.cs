using LgkProductions.Inspector.MetaData;

namespace LgkProductions.Inspector.Attributes;

public abstract class InspectorAttribute : Attribute
{
    public abstract void Apply(InspectorMember memberInfo, ref bool shouldInclude);
}
