using System;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class DoubleInput : NumberInput<double>
{
	protected override double StepSize { get; } = double.Epsilon;
}
