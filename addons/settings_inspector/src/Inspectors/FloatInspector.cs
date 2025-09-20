namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class FloatInspector : NumberInspector<float>
{
    protected override double StepSize { get; set; } = 10E-5;
}
