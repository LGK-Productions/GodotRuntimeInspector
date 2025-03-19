using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class ListInspector : MemberInspector
{
	[Export] private Button _addButton;
	[Export] private MenuButton _addMenuButton;
	[Export] private PackedScene _listElementScene;
	[Export] private Button? _expandButton;
	[Export] private Control _memberParent;

	private readonly List<ListElement> _listElements = new();
	private Type? _listElementType;
	private List<Type>? _assignableTypes;

	private IList? _list;

	protected override void OnInitialize()
	{
		_addButton.Pressed += AppendNewListElement;
		_expandButton.Toggled += ExpandButtonToggled;
		_addMenuButton.GetPopup().IndexPressed += AppendListElement;
	}

	protected override void OnRemove()
	{
		_addButton.Pressed -= AppendNewListElement;
		_expandButton.Toggled -= ExpandButtonToggled;
		_addMenuButton.GetPopup().IndexPressed -= AppendListElement;
	}

	private void ExpandButtonToggled(bool on)
	{
		_memberParent.Visible = on;
	}

	protected override object? GetValue()
	{
		if (_list is null) return null;
		_list.Clear();
		foreach (var inspector in _listElements)
		{
			if (inspector.TryRetrieveMember(out var value))
				_list.Add(value);
		}

		return _list;
	}

	protected override void SetValue(object value)
	{
		base.SetValue(value);
		if (value is not IList list) return;
		_list = list;
		_listElementType = list.GetType().GetGenericArguments()[0];
		_addButton.Visible = !(_listElementType.IsAbstract || _listElementType.IsInterface);
		_addMenuButton.Visible = _listElementType.IsAbstract || _listElementType.IsInterface;
		_expandButton?.SetVisible(false);
		if (_listElementType.IsAbstract || _listElementType.IsInterface)
		{
			var popupMenu = _addMenuButton.GetPopup();
			popupMenu.Clear();
			_assignableTypes = Util.GetAssignableTypes(_listElementType).ToList();
			foreach (var type in _assignableTypes)
			{
				popupMenu.AddItem(type.Name);
			}
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
		var listElementInstance = _listElementScene.Instantiate<ListElement>();
		var memberUiInfo = MemberUiInfo.Default;
		if (value.GetType() != _listElementType)
			memberUiInfo = memberUiInfo with { parentType = _listElementType };
		memberWrapper.MemberInspector.SetInstance(value, memberUiInfo);
		listElementInstance.SetMemberInspector(memberWrapper.MemberInspector, this);
		_memberParent.AddChild(listElementInstance);
		_listElements.Add(listElementInstance);
		_expandButton?.SetVisible(_listElements.Count > 0 && !LayoutFlags.IsSet(LayoutFlags.NotFoldable));
	}

	private void AppendListElement(Type? type)
	{
		if (type == null || _listElementType == null || !Util.TryCreateInstance(type, out var instance)) return;
		if (!_listElementType.IsAssignableFrom(type)) return;
		AppendListElement(instance);
	}

	private void AppendNewListElement() => AppendListElement(_listElementType);

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
		_expandButton?.SetVisible(_listElements.Count > 0 && !LayoutFlags.IsSet(LayoutFlags.NotFoldable));
	}

	public void MoveElement(ListElement element, bool up)
	{
		var index = _listElements.IndexOf(element);
		if (index < 0) return;
		var targetIndex = index + (up ? 1 : -1);
		if (targetIndex >= _listElements.Count || targetIndex < 0) return;
		_listElements.RemoveAt(index);
		_listElements.Insert(targetIndex, element);
		_memberParent.MoveChild(element, targetIndex);
	}

	public override void SetEditable(bool editable)
	{
		base.SetEditable(editable);
		foreach (var inspector in _listElements)
		{
			inspector.SetEditable(editable);
		}

		_addButton.Disabled = !editable;
	}

	private void OnChildValueChanged()
	{
		OnValueChanged();
	}

	protected override void OnSetMetaData(MetaDataMember member)
	{
		base.OnSetMetaData(member);
		_addButton.Disabled = member.IsReadOnly;
	}
}
