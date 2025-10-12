using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using Microsoft.Extensions.Logging;
using RuntimeInspector.Attributes;
using RuntimeInspector.Handlers;
using RuntimeInspector.Util;

namespace RuntimeInspector.Inspectors;

public abstract partial class MemberInspector : Control
{
    [Export] private Control? _background;

    private bool _initialized;
    [Export] private Label? _label;
    [Export] private MarginContainer? _marginContainer;
    private MemberWrapper? _wrapper;

    protected bool Editable = true;

    protected InspectorElement? Element;
    protected LayoutFlags LayoutFlags;
    protected MemberUiInfo MemberUiInfo;

    public Type? ValueType { get; protected set; }

    public void SetMember(InspectorElement iElement)
    {
        Element = iElement;
        OnSetMetaData(Element.MemberInfo);
        var memberUiInfo = MemberUiInfo.Default;
        ValueType = iElement.MemberInfo.Type;
        var isAssignableType = ValueType.IsAbstract || ValueType.IsInterface;
        if (isAssignableType)
        {
            memberUiInfo = memberUiInfo with { ParentType = iElement.MemberInfo.Type };
            var availableTypes = Util.Util.GetAssignableTypes(ValueType).ToArray();
            if (availableTypes.Length > 0) ValueType = availableTypes[0];
        }


        var value = Element.Value;
        if (value == null && !Util.Util.TryCreateInstance(ValueType, out value))
        {
            MemberInspectorHandler.Logger?.LogWarning("Value is null, could not create instance.");
            return;
        }

        SetInstance(value!, memberUiInfo, iElement.MemberInfo.LayoutFlags);

        Element.ValueChanged += UpdateMemberInputValue;
    }

    public void SetInstance(object value)
    {
        SetInstance(value, MemberUiInfo.Default);
    }

    public void SetInstance(object value, MemberUiInfo memberUiInfo, LayoutFlags flags = LayoutFlags.Default)
    {
        if (!_initialized)
            OnInitialize();

        ValueType = value.GetType();
        if (Element == null)
            _label?.SetText(ValueType.Name);

        SetMemberUiInfo(memberUiInfo);
        SetLayoutFlags(flags);
        SetValue(value);
    }


    protected virtual void SetMemberUiInfo(MemberUiInfo memberUiInfo)
    {
        MemberUiInfo = memberUiInfo;
    }

    protected virtual void SetLayoutFlags(LayoutFlags flags)
    {
        LayoutFlags = flags;
        _label?.SetVisible(!flags.IsSet(LayoutFlags.NoLabel));
        _background?.SetVisible(!flags.IsSet(LayoutFlags.NoBackground));
    }

    protected abstract object? GetValue();

    protected void SetValue(object value)
    {
        Clear();
        Callable.From(() => SetValueInternal(value)).CallDeferred();
    }

    protected virtual void SetValueInternal(object value)
    {
    }

    public virtual void SetEditable(bool editable)
    {
        Editable = editable;
    }

    protected virtual void OnSetMetaData(MetaDataMember member)
    {
        _label?.Text = Element.MemberInfo.DisplayName;
        _label?.TooltipText = Element.MemberInfo.Description;
        if (member.TryGetMetaData(LabelSizeAttribute.MetadataKey, out var labelSizeMultiplier))
            _label?.SizeFlagsStretchRatio = labelSizeMultiplier;
        _wrapper?.AddMargin((int)member.Spacing.Top, (int)member.Spacing.Botton, (int)member.Spacing.Left,
            (int)member.Spacing.Right);
        if (member.TryGetMetaData(LabelAttribute.TextKey, out var text))
            _wrapper?.SetLabel(text);
        if (member.TryGetMetaData(LabelAttribute.FontSizeKey, out var size))
            _wrapper?.SetLabelFontSize(size);
        _wrapper?.SetLine(member.HasLineAbove);


        SetEditable(!Element.MemberInfo.IsReadOnly);
    }

    public void SetWrapper(MemberWrapper wrapper)
    {
        _wrapper = wrapper;
    }

    protected virtual void Clear()
    {
    }

    protected virtual void OnInitialize()
    {
        _initialized = true;
    }

    protected virtual void OnRemove()
    {
        _initialized = false;
    }

    public void Remove()
    {
        Clear();
        if (Element != null)
        {
            Element.ValueChanged -= UpdateMemberInputValue;
            Element = null;
        }

        if (_initialized)
            OnRemove();
        QueueFree();
    }

    public event Action<ValueChangeTree>? ValueChanged;

    protected void OnValueChanged(ValueChangeTree valueChangeTree)
    {
        ValueChanged?.Invoke(valueChangeTree);
    }

    private void UpdateMemberInputValue(object instance, MetaDataMember member, object? value)
    {
        if (value == null) return;
        SetValue(value);
    }

    public bool TryRetrieveMember([NotNullWhen(true)] out object? result)
    {
        result = null;
        if (ValueType == null)
        {
            MemberInspectorHandler.Logger?.LogError("Could not retrieve member, due to no type being set");
            return false;
        }

        try
        {
            result = Convert.ChangeType(GetValue(), ValueType);
            return result != null;
        }
        catch (Exception e)
        {
            MemberInspectorHandler.Logger?.LogError(e, "Failed to convert value to type {valueType}", ValueType);
        }

        return false;
    }

    public bool TryRetrieveMember<T>([NotNullWhen(true)] out T? result)
    {
        result = default;
        if (ValueType != typeof(T))
        {
            MemberInspectorHandler.Logger?.LogError("Could not retrieve member, due to type not matching");
            return false;
        }

        try
        {
            result = (T?)Convert.ChangeType(GetValue(), typeof(T));
            return result != null;
        }
        catch (Exception e)
        {
            MemberInspectorHandler.Logger?.LogError(e, "Failed to convert value to type {valueType}", ValueType);
        }

        return false;
    }
}