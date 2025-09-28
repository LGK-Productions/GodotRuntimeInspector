using System;
using Godot;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class PathPickerAttribute(FileDialog.FileModeEnum type) : InspectorAttribute
{
    public static readonly MetaDataKey<FileDialog.FileModeEnum> PickerTypeKey = new("PickerType");
    public static readonly MetaDataKey<string[]> FilterKey = new("Extensions");

    public PathPickerAttribute(FileDialog.FileModeEnum type, string[] filters) : this(type)
    {
        Filters = filters;
    }

    public FileDialog.FileModeEnum Type { get; } = type;

    public string[] Filters { get; } = [];

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.SetMetaData(PickerTypeKey, Type);
        if (!Filters.IsEmpty())
            memberInfo.SetMetaData(FilterKey, Filters);
    }
}