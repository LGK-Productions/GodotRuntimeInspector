using System;
using System.Numerics;
using Godot;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class NumberInspector<T> : MemberInspector where T : struct, INumber<T>
{
	[Export] private SpinBox _spinBox;

	protected virtual double StepSize { get; } = 1;

	public override void _EnterTree()
	{
		base._EnterTree();
		_spinBox.ValueChanged += OnNumberChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_spinBox.ValueChanged -= OnNumberChanged;
	}

	protected override void SetValue(object value)
	{
        base.SetValue(value);
		_spinBox.Value = (double)Convert.ChangeType(value, typeof(double));
	}

	protected override object? GetValue()
	{
		return Convert.ChangeType(_spinBox.Value, typeof(T));
	}

	public override void SetEditable(bool editable)
	{
        base.SetEditable(editable);
		_spinBox.Editable = editable;
	}

	private void OnNumberChanged(double value)
	{
		OnValueChanged();
	}

	protected override void OnSetMetaData(MetaDataMember member)
	{
		if (member.MaxValue != null)
			_spinBox.MaxValue = (double)Convert.ChangeType(member.MaxValue, typeof(double));
		else
			_spinBox.AllowGreater = true;
		if (member.MinValue != null)
			_spinBox.MinValue = (double)Convert.ChangeType(member.MinValue, typeof(double));
		else
			_spinBox.AllowLesser = true;
		_spinBox.Step = StepSize;
	}

}
