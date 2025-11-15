using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using RuntimeInspector.Handlers;
using RuntimeInspector.Util;

namespace RuntimeInspector.Inspectors.Collections;

public partial class ArrayInspector : MemberInspector
{
    private readonly List<ListElement> _arrayElements = new();
    private List<Type>? _assignableTypes;
    [Export] private SpinBox? _elementCount;
    [Export] private FoldableContainer? _foldableContainer;

    private IList? _list;
    [Export] private PackedScene? _listElementScene;
    private Type? _listElementType;
    [Export] private Control? _memberParent;

    protected override void OnInitialize()
    {
        _elementCount.ValueChanged += ElementCountOnValueChanged;
    }

    private void ElementCountOnValueChanged(double value)
    {
        var previousCount = _arrayElements.Count;
        var diff = (int)value - previousCount;
        if (diff > 0)
            for (var i = 0; i < diff; i++)
                AppendListElement(_listElementType);
        else
            for (var i = 0; i < -diff; i++)
                RemoveListElement(_arrayElements[^1]);
    }

    protected override void OnRemove()
    {
        _elementCount.ValueChanged -= ElementCountOnValueChanged;
    }

    protected override object? GetValue()
    {
        if (_list is null) return null;
        _list.Clear();
        foreach (var inspector in _arrayElements)
            if (inspector.TryRetrieveMember(out var value))
                _list.Add(value);

        return _list as Array;
    }

    protected override void SetValueInternal(object value)
    {
        if (value is not IList list) return;
        _list = list;

        if (Element == null && _foldableContainer != null)
            _foldableContainer.Title = ValueType?.Name;

        _listElementType = ValueType.GetElementType();
        if (_listElementType.IsAbstract || _listElementType.IsInterface)
            _assignableTypes = Util.Util.GetAssignableTypes(_listElementType).ToList();

        foreach (var obj in _list)
        {
            if (obj == null) continue;
            AppendListElement(obj);
        }

        _elementCount.SetValueNoSignal(_list.Count);
    }

    protected override void Clear()
    {
        base.Clear();
        foreach (var element in _arrayElements)
        {
            element.ValueChanged -= OnChildValueChanged;
            element.Remove();
        }

        _arrayElements.Clear();
        _listElementType = null;
    }

    private void AppendListElement(object value)
    {
        if (_listElementType == null) return;
        var memberWrapper = MemberInspectorHandler.Instance?.MemberWrapperScene?.Instantiate<MemberWrapper>();
        memberWrapper.SetMemberType(_listElementType);
        var listElementInstance = _listElementScene!.Instantiate<ListElement>();
        var memberUiInfo = MemberUiInfo.Default;
        if (value.GetType() != _listElementType)
            memberUiInfo = memberUiInfo with { ParentType = _listElementType };
        memberWrapper.MemberInspector.SetInstance(value, memberUiInfo);
        listElementInstance.SetMemberInspector(memberWrapper.MemberInspector);
        _memberParent!.AddChild(listElementInstance);
        _arrayElements.Add(listElementInstance);
        listElementInstance.ValueChanged += OnChildValueChanged;
    }

    private void AppendListElement(Type? type)
    {
        if (type == null || _listElementType == null || !Util.Util.TryCreateInstance(type, out var instance)) return;
        if (!_listElementType.IsAssignableFrom(type)) return;
        AppendListElement(instance);
        OnValueChanged(new ValueChangeTree(this, _list));
    }

    private void AppendNewListElement()
    {
        AppendListElement(_listElementType);
    }

    private void AppendListElement(long index)
    {
        if (_assignableTypes == null) return;
        if (index < 0 || index >= _assignableTypes.Count) return;
        AppendListElement(_assignableTypes[(int)index]);
    }

    public void RemoveListElement(ListElement element)
    {
        var index = _arrayElements.IndexOf(element);
        if (index < 0) return;
        _arrayElements.RemoveAt(index);
        element.Remove();
        OnValueChanged(new ValueChangeTree(this, _list));
    }

    public void MoveElement(ListElement element, bool up)
    {
        var index = _arrayElements.IndexOf(element);
        if (index < 0) return;
        var targetIndex = index + (up ? 1 : -1);
        if (targetIndex >= _arrayElements.Count || targetIndex < 0) return;
        _arrayElements.RemoveAt(index);
        _arrayElements.Insert(targetIndex, element);
        _memberParent!.MoveChild(element, targetIndex);
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        foreach (var inspector in _arrayElements) inspector.SetEditable(editable);

        _elementCount!.Editable = editable;
    }

    private void OnChildValueChanged(ValueChangeTree tree)
    {
        OnValueChanged(new ValueChangeTree(this, _list, tree));
    }

    protected override void OnSetMetaData(MetaDataMember member)
    {
        base.OnSetMetaData(member);
        _elementCount!.Editable = !member.IsReadOnly;
        if (_foldableContainer != null)
        {
            _foldableContainer.Title = member.Name;
            _foldableContainer.TooltipText = member.Description;
        }
    }


    protected override void SetLayoutFlags(LayoutFlags flags)
    {
        base.SetLayoutFlags(flags);
        if (_foldableContainer != null)
        {
            _foldableContainer.Folded = !flags.IsSet(LayoutFlags.ExpandedInitially);
            if (flags.IsSet(LayoutFlags.NoLabel))
            {
                _foldableContainer.Title = "";
                _foldableContainer.TooltipText = "";
            }
        }

        if (flags.IsSet(LayoutFlags.NoBackground)) _memberParent?.GetParent()?.Reparent(this);


        if (flags.IsSet(LayoutFlags.NoElements)) _elementCount!.Visible = false;
    }
}