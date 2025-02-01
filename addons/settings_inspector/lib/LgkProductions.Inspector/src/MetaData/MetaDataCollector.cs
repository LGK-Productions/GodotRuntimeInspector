using LgkProductions.Inspector.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace LgkProductions.Inspector.MetaData;

public sealed class MetaDataCollector
{
    public static MetaDataCollection Collect<T>(CollectorOptions options)
        => Collect(typeof(T), options);

    public static MetaDataCollection Collect(Type type, CollectorOptions options)
    {
        if (type.IsPrimitive)
            throw new ArgumentException("Primitive types are not supported.");

        List<InspectorMember> members = [];
        foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (!TryCollectMember(member, options, out var memberInfo))
                continue;

            Debug.Assert(memberInfo.GetValue is not null);

            members.Add(memberInfo);
        }

        return new(members);
    }

    static bool TryCollectMember(MemberInfo member, CollectorOptions options, [MaybeNullWhen(false)] out InspectorMember memberInfo)
    {
        memberInfo = null;

        switch (member)
        {
            case FieldInfo field:
                if (!TryCollect(member, field.FieldType, field.IsPublic && options.IncludeFields, options, out memberInfo))
                    return false;

                memberInfo.GetValue = field.GetValue;

                if (!field.IsInitOnly)
                    memberInfo.SetValue = field.SetValue;
                else
                    memberInfo.IsReadOnly = true;
                return true;

            case PropertyInfo property:
                if (!property.CanRead)
                    return false;

                if (!TryCollect(member, property.PropertyType, property.GetAccessors().Any(x => x.IsPublic), options, out memberInfo))
                    return false;

                memberInfo.GetValue = property.GetValue;

                if (property.CanWrite)
                    memberInfo.SetValue = property.SetValue;
                else
                    memberInfo.IsReadOnly = true;
                return true;

            default:
                return false;
        }

        static bool TryCollect(MemberInfo member, Type typeOfValue, bool shouldInclude, CollectorOptions options, [MaybeNullWhen(false)] out InspectorMember memberInfo)
        {
            memberInfo = new(member.Name, typeOfValue, member.DeclaringType);

            foreach (var attribute in member.GetCustomAttributes())
            {
                ApplyAttribute(attribute, memberInfo, ref shouldInclude);
            }

            return shouldInclude;
        }
    }

    static void ApplyAttribute(object attribute, InspectorMember memberInfo, ref bool shouldInclude)
    {
        switch (attribute)
        {
            case BrowsableAttribute browsable:
                shouldInclude = browsable.Browsable;
                break;

            case CategoryAttribute category:
                memberInfo.GroupName = category.Category;
                break;

            case DefaultValueAttribute defaultValue:
                memberInfo.DefaultValue = defaultValue.Value;
                break;

            case DescriptionAttribute description:
                memberInfo.Description = description.Description;
                break;

            case DisplayNameAttribute displayName:
                memberInfo.DisplayName = displayName.DisplayName;
                break;

            case ReadOnlyAttribute readOnly:
                memberInfo.IsReadOnly = readOnly.IsReadOnly;
                break;

            case DisplayAttribute display:
                memberInfo.DisplayName = display.GetName() ?? memberInfo.Name;
                memberInfo.Description ??= display.GetDescription();
                memberInfo.GroupName ??= display.GetGroupName();
                memberInfo.Order ??= display.GetOrder();
                break;

            case EditableAttribute editable:
                memberInfo.IsReadOnly = editable.AllowEdit;
                break;

            case MaxLengthAttribute maxLength:
                memberInfo.MaxLength = maxLength.Length;
                break;

            case MinLengthAttribute minLength:
                memberInfo.MinLength = minLength.Length;
                break;

            case RangeAttribute range:
                memberInfo.MinValue = range.Minimum;
                memberInfo.MaxValue = range.Maximum;
                break;

            case RequiredAttribute:
                memberInfo.IsRequired = true;
                break;

            case StringLengthAttribute stringLength:
                memberInfo.MinLength = stringLength.MinimumLength;
                memberInfo.MaxLength = stringLength.MaximumLength;
                break;

            case InspectorAttribute inspectorAttribute:
                inspectorAttribute.Apply(memberInfo, ref shouldInclude);
                break;
        }
    }
}
