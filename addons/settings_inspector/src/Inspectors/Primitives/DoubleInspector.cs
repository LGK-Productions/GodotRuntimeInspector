namespace SettingInspector.addons.settings_inspector.Inspectors.Primitives;

public partial class DoubleInspector : NumberInspector<double>
{
    protected override double StepSize { get; set; } = 10E-10;
}