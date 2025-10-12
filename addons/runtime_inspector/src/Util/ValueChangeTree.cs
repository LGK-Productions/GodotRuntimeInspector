using RuntimeInspector.Inspectors;

namespace RuntimeInspector.Util;

public class ValueChangeTree(MemberInspector member, object newValue, ValueChangeTree? child = null)
{
    public ValueChangeTree? Child { get; } = child;
    public MemberInspector Member { get; } = member;
    public object NewValue { get; } = newValue;

    public override string ToString()
    {
        return Child == null
            ? $"{Member.ValueType.Name}: {NewValue}"
            : $"{Member.ValueType.Name}: {NewValue}\n{Child}";
    }
}