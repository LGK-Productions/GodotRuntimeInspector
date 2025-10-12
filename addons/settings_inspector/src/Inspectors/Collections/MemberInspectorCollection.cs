using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector;
using SettingInspector.Handlers;
using SettingInspector.Util;

namespace SettingInspector.Inspectors.Collections;

public partial class MemberInspectorCollection : Control, IMemberInspectorCollection
{
    private readonly List<(InspectorElement, MemberInspector)> _inspectors = new();
    private readonly Dictionary<string, MemberGroup> _memberGroups = new();
    [Export] private PackedScene? _boxGroupScene;
    [Export] private Node? _memberInspectorParent;
    [Export] private ScrollContainer? _scrollContainer;

    public void SetMemberInspector(Inspector inspector)
    {
        Clear();
        foreach (var element in inspector.Elements) AddElement(element);
    }

    public void AddElement(InspectorElement element)
    {
        var memberWrapper = MemberInspectorHandler.Instance?.MemberWrapperScene?.Instantiate<MemberWrapper>();
        memberWrapper.SetMemberType(element.MemberInfo.Type);

        //Grouping Logic
        var groupLayout = element.MemberInfo.Group;
        var groupScene = _boxGroupScene;

        if (groupLayout == null)
        {
            _memberInspectorParent!.AddChild(memberWrapper);
        }
        else
        {
            if (!_memberGroups.TryGetValue(groupLayout.Title, out var group))
            {
                var memberGroupNode = groupScene!.Instantiate();
                _memberInspectorParent!.AddChild(memberGroupNode);
                group = (MemberGroup)memberGroupNode;
                group.SetGroup(groupLayout);
                _memberGroups.Add(groupLayout.Title, group);
            }

            group.AddMember(memberWrapper);
        }

        memberWrapper.MemberInspector.SetMember(element);
        _inspectors.Add((element, memberWrapper.MemberInspector));
        memberWrapper.MemberInspector.ValueChanged += OnChildValueChanged;
    }

    public void WriteBack()
    {
        foreach (var (element, inspector) in _inspectors)
        {
            if (element.MemberInfo.IsReadOnly) continue;
            if (inspector.TryRetrieveMember(out var value))
                element.Value = value;
        }
    }

    public void Remove()
    {
        Clear();
        QueueFree();
    }

    public void SetEditable(bool editable)
    {
        foreach (var (_, memberInspector) in _inspectors) memberInspector.SetEditable(editable);
    }

    public IEnumerator<(InspectorElement, MemberInspector)> GetEnumerator()
    {
        return _inspectors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public event Action<ValueChangeTree>? ValueChanged;

    public void SetScrollable(bool scrollable)
    {
        var parent = _memberInspectorParent!.GetParent();
        _memberInspectorParent.Owner = null;
        if (!scrollable)
        {
            _scrollContainer!.Visible = false;
            if (parent is not ScrollContainer) return;
            parent.RemoveChild(_memberInspectorParent);
            parent.GetParent().AddChild(_memberInspectorParent);
        }
        else
        {
            _scrollContainer!.Visible = true;
            if (parent is ScrollContainer) return;
            parent.RemoveChild(_memberInspectorParent);
            _scrollContainer.AddChild(_memberInspectorParent);
        }

        _memberInspectorParent.Owner = _memberInspectorParent.GetParent();
    }

    private void Clear()
    {
        foreach (var (_, inspector) in _inspectors)
        {
            inspector.ValueChanged -= OnChildValueChanged;
            inspector.Remove();
        }

        _inspectors.Clear();
        foreach (var (_, group) in _memberGroups) group.QueueFree();

        _memberGroups.Clear();
    }

    private void OnChildValueChanged(ValueChangeTree tree)
    {
        ValueChanged?.Invoke(tree);
    }
}