using System;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class ComplexInput : Control, IMemberInput
{
	[Export] Button _button;
	
	private object? _instance;
	private InspectorElement _element;

	public override void _EnterTree()
	{
		_button.Pressed += OnPressed;
	}

	public override void _ExitTree()
	{
		_button.Pressed -= OnPressed;
	}

	public void SetValue(object value)
	{
		_instance = value;
	}

	public object GetValue()
	{
		return _instance;
	}

	public void SetEditable(bool editable)
	{
		_button.Disabled = !editable;
	}

	private async void OnPressed()
	{
		try
		{
			if (_instance == null)
			{
				_instance = Activator.CreateInstance(_element.MemberInfo.Type);
			}
			_instance = await ClassInspectorHandler.Instance.OpenClassInspector(_instance, true);
			OnValueChanged?.Invoke(_instance);
		}
		catch (OperationCanceledException e)
		{
		}
	}

	void IMemberInput.OnSetElement(InspectorElement element)
	{
		_button.Text = $"Edit {element.MemberInfo.DisplayName}";
		_instance = element.Value;
		_element = element;
	}
	
	public event Action<object>? OnValueChanged;
}
