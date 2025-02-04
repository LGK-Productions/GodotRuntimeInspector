using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class ListInspector : MemberInspector
{
	[Export] private Button _addButton;
	[Export] private Node _memberParent;

	private readonly List<MemberInspector> _inspectors = new();
	private Type? _listElementType;
	private PackedScene? _listElementScene;

	private IList? _list;

	public override void _EnterTree()
	{
		base._EnterTree();
		_addButton.Pressed += AddNewListElement;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_addButton.Pressed -= AddNewListElement;
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

	protected override void SetValue(object? value)
	{
		ClearInspectors();
		if (value == null)
		{
			try
			{
				value = Activator.CreateInstance(ValueType);
			}
			catch (Exception e)
			{
				GD.PrintErr(e);
				return;
			}
		}
		if (value is not IList list) return;
		_list = list;
		_listElementType = list.GetType().GetGenericArguments()[0];
		_listElementScene = MemberInspectorHandler.Instance.GetInputScene(_listElementType);
		foreach (var obj in _list)
		{
			AddListElement(obj);
		}
	}

	private void AddListElement(object? value)
	{
		if (_listElementScene == null) return;
		var memberInstance = _listElementScene.Instantiate<MemberInspector>();
		memberInstance.SetInstance(value, new MemberUiInfo() {IsLabelHidden = true});
		_memberParent.AddChild(memberInstance);
		_inspectors.Add(memberInstance);
	}

	private void AddNewListElement()
	{
		if (_listElementType == null) return;
		try
		{
			var value = Activator.CreateInstance(_listElementType);
			AddListElement(value);
		}
		catch (Exception e)
		{
			GD.PrintErr(e.ToString());
		}
		
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

	private void ClearInspectors()
	{
		foreach (var inspector in _inspectors)
		{
			inspector.ValueChanged -= OnChildValueChanged;
			inspector.QueueFree();
		}
		_inspectors.Clear();
		_listElementType = null;
		_listElementScene = null;
	}

	private void OnChildValueChanged()
	{
		OnValueChanged();
	}
	protected override void OnSetMetaData(MetaDataMember member)
	{
		_addButton.Disabled = member.IsReadOnly;
	}
}
