using System;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class DoubleInspector : NumberInspector<double>
{
	protected override double StepSize { get; } = double.Epsilon;
}
