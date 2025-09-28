using System;
using LgkProductions.Inspector.Attributes;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class LabelAttribute(string text, int fontSize = 20) : InspectorAttribute
{
    public static readonly MetaDataKey<string> TextKey = new("LabelText");
    public static readonly MetaDataKey<int> FontSizeKey = new("LabelFontSize");

    public string Text { get; } = text;
    public int FontSize { get; } = fontSize;

    public override void Apply(MetaDataMember memberInfo, ref bool shouldInclude)
    {
        memberInfo.SetMetaData(TextKey, Text);
        memberInfo.SetMetaData(FontSizeKey, FontSize);
    }
}