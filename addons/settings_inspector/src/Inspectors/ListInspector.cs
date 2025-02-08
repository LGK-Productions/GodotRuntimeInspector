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
	[Export] private PackedScene _listElementScene;
	[Export] private Button _expandButton;
	[Export] private Control _memberParent;
	[Export] private PopupMenu _popupMenu;

	private readonly List<MemberInspector> _inspectors = new();
	private Type? _listElementType;
	private PackedScene? _listInspectorScene;

	private IList? _list;

	public override void _EnterTree()
	{
		base._EnterTree();
		_addButton.Pressed += AppendNewListElement;
		_expandButton.Toggled += ExpandButtonToggled;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_addButton.Pressed -= AppendNewListElement;
		_expandButton.Toggled -= ExpandButtonToggled;
	}

	private void ExpandButtonToggled(bool on)
	{
		_memberParent.Visible = on;
	}

	protected override object? GetValue()
	{
		if (_list is null) return null;
		_list.Clear();
		foreach (var inspector in _inspectors)
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
		_listInspectorScene = MemberInspectorHandler.Instance.GetInputScene(_listElementType);

		foreach (var obj in _list)
		{
			if (obj == null) continue;
			AddListElement(obj);
		}
	}

	protected override void Clear()
	{
		base.Clear();
		foreach (var inspector in _inspectors)
		{
			inspector.ValueChanged -= OnChildValueChanged;
			inspector.Remove();
		}

		_inspectors.Clear();
		_listElementType = null;
		_listInspectorScene = null;
	}

	private void AddListElement(object value)
	{
		if (_listInspectorScene == null) return;
		var memberInstance = _listInspectorScene.Instantiate<MemberInspector>();
		var listElementInstance = _listElementScene.Instantiate<ListElement>();
		memberInstance.SetInstance(value, new MemberUiInfo() { IsLabelHidden = true });
		listElementInstance.SetUi(memberInstance);
		_memberParent.AddChild(listElementInstance);
		_inspectors.Add(memberInstance);
	}

	private void AppendNewListElement()
	{
		if (_listElementType == null) return;
		try
		{
			object? value = null;
			if (_listElementType.IsInterface || _listElementType.IsAbstract)
			{
				_popupMenu.Clear();
				var types = GetAssignableTypes(_listElementType).ToList();
				foreach (var type in types)
				{
					_popupMenu.AddItem(type.Name);
				}

				_popupMenu.Position = (Vector2I)GetScreenPosition() + new Vector2I(0, (int)Size.Y);
				_popupMenu.IndexPressed += IndexPressed;
				_popupMenu.Popup();

				void IndexPressed(long index)
				{
					_popupMenu.IndexPressed -= IndexPressed;
					if (types.Count <= index) return;
					AddElement(types[(int)index]);
				}
			}
			else
				AddElement(_listElementType);
		}
		catch (Exception e)
		{
			GD.PrintErr(e.ToString());
		}

		void AddElement(Type type)
		{
			var value = Activator.CreateInstance(type);
			if (value != null)
				AddListElement(value);
		}
	}

	private void RemoveListElement(int index)
	{
		if (_inspectors.Count <= index || index < 0) return;
		var inspector = _inspectors[index];
		_inspectors.RemoveAt(index);
		inspector.QueueFree();
	}

	private void RemoveLastListElement()
	{
		RemoveListElement(_inspectors.Count - 1);
	}

	public override void SetEditable(bool editable)
	{
		base.SetEditable(editable);
		foreach (var inspector in _inspectors)
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
