namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class IntInspector : NumberInspector<int>
{
    protected override double StepSize { get; set; } = 1;
}