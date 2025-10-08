using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.Inspectors.InspectorCollections;
using SettingInspector.addons.settings_inspector.ValueTree;

namespace SettingInspector.addons.settings_inspector.Inspectors;

public partial class ListInspector : MemberInspector
{
    private readonly List<ListElement> _listElements = new();
    [Export] private Button? _addButton;
    [Export] private MenuButton? _addMenuButton;
    private List<Type>? _assignableTypes;
    [Export] private FoldableContainer? _foldableContainer;

    private IList? _list;
    [Export] private PackedScene? _listElementScene;
    private Type? _listElementType;
    [Export] private Control? _memberParent;

    protected override void OnInitialize()
    {
        _addButton!.Pressed += AppendNewListElement;
        _addMenuButton!.GetPopup().IndexPressed += AppendListElement;
    }

    protected override void OnRemove()
    {
        _addButton!.Pressed -= AppendNewListElement;
        _addMenuButton!.GetPopup().IndexPressed -= AppendListElement;
    }

    protected override object? GetValue()
    {
        if (_list is null) return null;
        _list.Clear();
        foreach (var inspector in _listElements)
            if (inspector.TryRetrieveMember(out var value))
                _list.Add(value);

        return _list;
    }

    protected override void SetValueInternal(object value)
    {
        if (value is not IList list) return;
        _list = list;

        if (Element == null)
            _foldableContainer?.Title = ValueType?.Name;

        _listElementType = list.GetType().GetGenericArguments()[0];
        _addButton!.Visible = !(_listElementType.IsAbstract || _listElementType.IsInterface);
        _addMenuButton!.Visible = _listElementType.IsAbstract || _listElementType.IsInterface;
        if (_listElementType.IsAbstract || _listElementType.IsInterface)
        {
            var popupMenu = _addMenuButton.GetPopup();
            popupMenu.Clear();
            _assignableTypes = Util.GetAssignableTypes(_listElementType).ToList();
            foreach (var type in _assignableTypes) popupMenu.AddItem(type.Name);
        }

        foreach (var obj in _list)
        {
            if (obj == null) continue;
            AppendListElement(obj);
        }
    }

    protected override void Clear()
    {
        base.Clear();
        foreach (var element in _listElements)
        {
            element.ValueChanged -= OnChildValueChanged;
            element.Remove();
        }

        _listElements.Clear();
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
        listElementInstance.SetMemberInspector(memberWrapper.MemberInspector, this);
        _memberParent!.AddChild(listElementInstance);
        _listElements.Add(listElementInstance);
        listElementInstance.ValueChanged += OnChildValueChanged;
    }

    private void AppendListElement(Type? type)
    {
        if (type == null || _listElementType == null || !Util.TryCreateInstance(type, out var instance)) return;
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
        var index = _listElements.IndexOf(element);
        if (index < 0) return;
        _listElements.RemoveAt(index);
        element.Remove();
        OnValueChanged(new ValueChangeTree(this, _list));
    }

    public void MoveElement(ListElement element, bool up)
    {
        var index = _listElements.IndexOf(element);
        if (index < 0) return;
        var targetIndex = index + (up ? 1 : -1);
        if (targetIndex >= _listElements.Count || targetIndex < 0) return;
        _listElements.RemoveAt(index);
        _listElements.Insert(targetIndex, element);
        _memberParent!.MoveChild(element, targetIndex);
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        foreach (var inspector in _listElements) inspector.SetEditable(editable);

        _addButton!.Disabled = !editable;
    }

    private void OnChildValueChanged(ValueChangeTree tree)
    {
        OnValueChanged(new ValueChangeTree(this, _list, tree));
    }

    protected override void OnSetMetaData(MetaDataMember member)
    {
        base.OnSetMetaData(member);
        _addButton!.Disabled = member.IsReadOnly;
        _foldableContainer?.Title = member.Name;
        _foldableContainer?.TooltipText = member.Description;
    }


    protected override void SetLayoutFlags(LayoutFlags flags)
    {
        base.SetLayoutFlags(flags);
        _foldableContainer?.Folded = !flags.IsSet(LayoutFlags.ExpandedInitially);

        if (flags.IsSet(LayoutFlags.NoBackground)) _memberParent?.GetParent()?.Reparent(this);

        if (flags.IsSet(LayoutFlags.NoLabel))
        {
            _foldableContainer?.Title = "";
            _foldableContainer?.TooltipText = "";
        }

        if (flags.IsSet(LayoutFlags.NoElements)) _addButton!.Visible = false;
    }
}