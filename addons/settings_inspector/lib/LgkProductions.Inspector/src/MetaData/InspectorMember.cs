using LgkProductions.Inspector.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace LgkProductions.Inspector.MetaData;

[DebuggerDisplay("{DisplayName}")]
public sealed partial class InspectorMember(string name, Type typeOfValue, Type declaringType)
{
    /// <summary>
    /// See <see cref="FieldInfo.FieldType"/> or <see cref="PropertyInfo.PropertyType"/>
    /// </summary>
    public Type Type { get; } = typeOfValue;

    /// <summary>
    /// See <see cref="MemberInfo.DeclaringType"/>
    /// </summary>
    public Type DeclaringType { get; } = declaringType;

    /// <summary>
    /// See <see cref="MemberInfo.Name"/>
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// See <see cref="DisplayNameAttribute"/> and <see cref="DisplayAttribute.Name"/>
    /// </summary>
    public string DisplayName { get; set; } = name;

    /// <summary>
    /// See <see cref="DescriptionAttribute"/> and <see cref="DisplayAttribute.Description"/>
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// See <see cref="CategoryAttribute"/> and <see cref="DisplayAttribute.GroupName"/>
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// See <see cref="DefaultValueAttribute"/>
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// See <see cref="ReadOnlyAttribute"/>
    /// </summary>
    public bool IsReadOnly { get; set; }

    public GetValueDelegate GetValue { get; set; } = (obj) => throw new NotImplementedException();
    public SetValueDelegate SetValue { get; set; } = (obj, value) => throw new NotImplementedException();

    /// <summary>
    /// See <see cref="PropertyOrderAttribute"/> and <see cref="DisplayAttribute.Order"/>
    /// </summary>
    public float? Order { get; set; }

    /// <summary>
    /// See <see cref="StringLengthAttribute.MinimumLength"/> and <see cref="MinLengthAttribute"/>
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// See <see cref="StringLengthAttribute.MaximumLength"/> and <see cref="MaxLengthAttribute"/>
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// See <see cref="RangeAttribute.Minimum"/>
    /// </summary>
    public object? MinValue { get; set; }

    /// <summary>
    /// See <see cref="RangeAttribute.Maximum"/>
    /// </summary>
    public object? MaxValue { get; set; }

    /// <summary>
    /// See <see cref="RequiredAttribute"/>
    /// </summary>
    public bool IsRequired { get; set; }

    public bool IsDeclaredIn(object instance)
        => DeclaringType.IsAssignableFrom(instance.GetType());
}

public delegate object? GetValueDelegate(object obj);
public delegate void SetValueDelegate(object obj, object? value);
