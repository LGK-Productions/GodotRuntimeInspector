namespace SettingInspector.addons.settings_inspector.Inspectors;

public partial class FloatInspector : NumberInspector<float>
{
    protected override double StepSize { get; set; } = 10E-5;
}