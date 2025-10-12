namespace SettingInspector.Inspectors.Primitives;

public partial class FloatInspector : NumberInspector<float>
{
    protected override double StepSize { get; set; } = 10E-5;
}