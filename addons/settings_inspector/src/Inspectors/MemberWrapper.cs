using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace SettingInspector.Inspectors;

public partial class MemberWrapper : MarginContainer
{
    [Export] private PackedScene? _defaultInspectorScene;
    [Export] private Array<PackedScene?> _inspectorScenes = new();
    [Export] private Label? _label;

    [Export] private Control? _memberContainer;
    [Export] private Panel? _panel;

    public MemberInspector? MemberInspector { get; private set; }

    public MemberInspector SetMemberType(Type type)
    {
        MemberInspector = GetInputScene(type)!.Instantiate<MemberInspector>();
        _memberContainer?.AddChild(MemberInspector);
        MemberInspector.SetWrapper(this);
        return MemberInspector;
    }

    public void AddMargin(int top, int bottom, int left, int right)
    {
        if (_memberContainer == null) return;
        _memberContainer.AddThemeConstantOverride("margin_top", top);
        _memberContainer.AddThemeConstantOverride("margin_bottom", bottom);
        _memberContainer.AddThemeConstantOverride("margin_left", left);
        _memberContainer.AddThemeConstantOverride("margin_right", right);
    }

    public void SetMargin(int? top = null, int? bottom = null, int? left = null, int? right = null)
    {
        if (top.HasValue)
            AddThemeConstantOverride("margin_top", top.Value);
        if (bottom.HasValue)
            AddThemeConstantOverride("margin_bottom", bottom.Value);
        if (left.HasValue)
            AddThemeConstantOverride("margin_left", left.Value);
        if (right.HasValue)
            AddThemeConstantOverride("margin_right", right.Value);
    }

    public void SetLabel(string text)
    {
        if (_label == null) return;
        _label.Text = text;
        _label.Visible = true;
    }

    public void SetLabelFontSize(int fontSize)
    {
        _label?.AddThemeFontSizeOverride("font_size", fontSize);
    }

    public void SetLine(bool visible = true)
    {
        if (_panel == null) return;
        _panel.Visible = visible;
    }

    private PackedScene? GetInputScene(Type inputType)
    {
        if (inputType == typeof(bool))
            if (_inspectorScenes.Count > 0)
                return _inspectorScenes[0];
        if (inputType == typeof(int))
            if (_inspectorScenes.Count > 1)
                return _inspectorScenes[1];
        if (inputType == typeof(float))
            if (_inspectorScenes.Count > 2)
                return _inspectorScenes[2];
        if (inputType == typeof(double))
            if (_inspectorScenes.Count > 3)
                return _inspectorScenes[3];
        if (inputType.IsEnum)
            if (_inspectorScenes.Count > 4)
                return _inspectorScenes[4];
        if (inputType == typeof(Color))
            if (_inspectorScenes.Count > 7)
                return _inspectorScenes[7];

        if (inputType.IsGenericType && inputType.GetGenericTypeDefinition() == typeof(List<>))
            if (_inspectorScenes.Count > 6)
                return _inspectorScenes[6];
        if (inputType.IsArray)
            if (_inspectorScenes.Count > 8)
                return _inspectorScenes[8];
        if (!inputType.IsPrimitive && inputType != typeof(string))
            if (_inspectorScenes.Count > 5)
                return _inspectorScenes[5];

        return _defaultInspectorScene;
    }
}