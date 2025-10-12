namespace RuntimeInspector.Inspectors.Primitives;

public partial class IntInspector : NumberInspector<int>
{
    protected override double StepSize { get; set; } = 1;
}