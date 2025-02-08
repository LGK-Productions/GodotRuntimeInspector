using Godot;
using System;
using System.Linq;
using SettingInspector.addons.settings_inspector.src;
using SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class ListElement : Control
{
	[Export] private Button _upButton;
	[Export] private Button _downButton;
	[Export] private Button _deleteButton;
	[Export] private Control _inspectorContainer;
	
	private MemberInspector? _inspector;
	private ListInspector? _listInspector;

	public void SetMemberInspector(MemberInspector inspector, ListInspector listInspector)
	{
		_inspector = inspector;
		_listInspector = listInspector;
		
		if (_inspector.GetParent() != null)
			_inspector.Reparent(_inspectorContainer);
		else
			_inspectorContainer.AddChild(_inspector);
		_inspector.AddThemeConstantOverride("margin_top", 0);
		_inspector.AddThemeConstantOverride("margin_left", 0);
		_inspector.AddThemeConstantOverride("margin_right", 0);
		_inspector.AddThemeConstantOverride("margin_bottom", 0);
		
		_inspector.ValueChanged += OnMemberValueChanged;
		_upButton.Pressed += OnUpPressed;
		_downButton.Pressed += OnDownPressed;
		_deleteButton.Pressed += OnDeletePressed;
	}

	private void OnUpPressed()
	{
		_listInspector?.MoveElement(this, false);
	}

	private void OnDownPressed()
	{
		_listInspector?.MoveElement(this, true);
	}

	private void OnDeletePressed()
	{
		_listInspector?.RemoveListElement(this);
	}
	
	public bool TryRetrieveMember(out object? value)
	{
		value = null;
		return _inspector != null && _inspector.TryRetrieveMember(out value);
	}

	public void Remove()
	{
		Clear();
		QueueFree();
	}

	public void SetEditable(bool editable)
	{
		_inspector?.SetEditable(editable);
	}

	private void Clear()
	{
		_inspector?.Remove();
		_inspector = null;
		_upButton.Pressed -= OnUpPressed;
		_downButton.Pressed -= OnDownPressed;
		_deleteButton.Pressed -= OnDeletePressed;
	}

	private void OnMemberValueChanged()
	{
		ValueChanged?.Invoke();
	}
	
	public event Action ValueChanged;
}
