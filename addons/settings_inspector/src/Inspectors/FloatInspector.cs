using SettingInspector.addons.settings_inspector.src.InputControllers;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class FloatInspector : NumberInspector<float>
{
	protected override double StepSize { get; } = float.Epsilon;
}
