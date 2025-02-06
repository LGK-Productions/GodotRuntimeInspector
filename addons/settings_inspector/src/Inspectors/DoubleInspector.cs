namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class DoubleInspector : NumberInspector<double>
{
	protected override double StepSize { get; } = double.Epsilon;
}
