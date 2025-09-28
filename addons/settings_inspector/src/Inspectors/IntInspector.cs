namespace SettingInspector.addons.settings_inspector.Inspectors;

public partial class IntInspector : NumberInspector<int>
{
    protected override double StepSize { get; set; } = 1;
}