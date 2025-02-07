using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class ListInspector : MemberInspector
{
	[Export] private Button _addButton;
	[Export] private Button _removeButton;
	[Export] private Button _expandButton;
	[Export] private Control _memberParent;

	private readonly List<MemberInspector> _inspectors = new();
	private Type? _listElementType;
	private PackedScene? _listElementScene;

	private IList? _list;

	public override void _EnterTree()
	{
		base._EnterTree();
		_addButton.Pressed += AppendNewListElement;
		_removeButton.Pressed += RemoveLastListElement;
		_expandButton.Toggled += ExpandButtonToggled;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_addButton.Pressed -= AppendNewListElement;
		_removeButton.Pressed -= RemoveLastListElement;
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
		_listElementScene = MemberInspectorHandler.Instance.GetInputScene(_listElementType);
        
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
		_listElementScene = null;
	}

	private void AddListElement(object value)
	{
		if (_listElementScene == null) return;
		var memberInstance = _listElementScene.Instantiate<MemberInspector>();
		memberInstance.SetInstance(value, new MemberUiInfo() {IsLabelHidden = true});
		_memberParent.AddChild(memberInstance);
		_inspectors.Add(memberInstance);
	}

	private void AppendNewListElement()
	{
		if (_listElementType == null) return;
		try
		{
			var value = Activator.CreateInstance(_listElementType);
			if (value != null)
				AddListElement(value);
		}
		catch (Exception e)
		{
			GD.PrintErr(e.ToString());
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
}
