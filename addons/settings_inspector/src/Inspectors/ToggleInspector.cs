using Godot;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class ToggleInspector : MemberInspector
{
	[Export] Button _checkbox;

	protected override void OnInitialize()
	{
		_checkbox.Pressed += OnPressed;
	}

	protected override void OnRemove()
	{
		_checkbox.Pressed -= OnPressed;
	}

	protected override void SetValue(object value)
	{
		base.SetValue(value);
		_checkbox.SetPressed((bool)value);
	}

	protected override object? GetValue()
	{
		return _checkbox.IsPressed();
	}

	public override void SetEditable(bool editable)
	{
		base.SetEditable(editable);
		_checkbox.Disabled = !editable;
	}

	private void OnPressed()
	{
		OnValueChanged();
	}
}
