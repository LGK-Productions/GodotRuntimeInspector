using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class ListInspector : MemberInspector
{
	[Export] private Button _addButton;
	[Export] private MenuButton _addMenuButton;
	[Export] private PackedScene _listElementScene;
	[Export] private Button _expandButton;
	[Export] private Control _memberParent;

	private readonly List<ListElement> _listElements = new();
	private Type? _listElementType;
	private List<Type>? _assignableTypes;
	private PackedScene? _listInspectorScene;

	private IList? _list;

	public override void _EnterTree()
	{
		base._EnterTree();
		_addButton.Pressed += AppendNewListElement;
		_expandButton.Toggled += ExpandButtonToggled;
		_addMenuButton.GetPopup().IndexPressed += AppendListElement;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
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
		if (_listElementType.IsAbstract || _listElementType.IsInterface)
		{
			var popupMenu = _addMenuButton.GetPopup();
			popupMenu.Clear();
			_assignableTypes = GetAssignableTypes(_listElementType).ToList();
			foreach (var type in _assignableTypes)
			{
				popupMenu.AddItem(type.Name);
			}
		}
		_listInspectorScene = MemberInspectorHandler.Instance.GetInputScene(_listElementType);

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
		_listInspectorScene = null;
	}

	private void AppendListElement(object value)
	{
		if (_listInspectorScene == null) return;
		var memberInstance = _listInspectorScene.Instantiate<MemberInspector>();
		var listElementInstance = _listElementScene.Instantiate<ListElement>();
		memberInstance.SetInstance(value, MemberUiInfo.Default);
		listElementInstance.SetMemberInspector(memberInstance, this);
		_memberParent.AddChild(listElementInstance);
		_listElements.Add(listElementInstance);
	}

	private void AppendListElement(Type? type)
	{
		if (type == null) return;
		try
		{
			var value = Activator.CreateInstance(type);
			if (value != null)
				AppendListElement(value);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
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

	private IEnumerable<Type> GetAssignableTypes(Type type)
	{
		return AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
			.Where(p => p is { IsAbstract: false, IsInterface: false } && type.IsAssignableFrom(p));
	}
}
