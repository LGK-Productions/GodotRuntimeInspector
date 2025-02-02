using System;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class IntInput : NumberInput<int>
{
	protected override double StepSize { get; } = 1;
}
