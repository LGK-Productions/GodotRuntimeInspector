using SettingInspector.addons.settings_inspector.src.InputControllers;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class IntInspector : NumberInspector<int>
{
	protected override double StepSize { get; } = 1;
}
