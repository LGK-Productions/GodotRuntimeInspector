using System;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class LabelAttribute(string text, int fontSize = 17) : InspectorAttribute
{
    public const string TextKey = "LabelText";
    public const string FontSizeKey = "LabelFontSize";

    public string Text { get; } = text;
    public int FontSize { get; } = fontSize;

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.CustomMetaData.Add(TextKey, text);
        memberInfo.CustomMetaData.Add(FontSizeKey, FontSize);
    }
}