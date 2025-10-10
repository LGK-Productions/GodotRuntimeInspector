using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.Attributes;
using SettingInspector.addons.settings_inspector.Attributes;
using Orientation = LgkProductions.Inspector.Orientation;
using Timer = System.Timers.Timer;

namespace SettingInspector.addons.settings_inspector.Testing;

[Serializable]
internal class TestModel : ITickProvider
{
    public TestModel()
    {
        var timer = new Timer(1000);
        timer.Elapsed += (sender, args) => TestUpdates++;
        timer.Start();
    }

    #region Primitives

    [Tab("Primitives")] public bool TestBool { get; set; }

    [Tab("Primitives")] [Checkbox] public bool TestCheckbox { get; set; }

    [Tab("Primitives")] public int TestInt { get; set; }

    [Tab("Primitives")] public float TestFloat { get; set; }

    [Tab("Primitives")] public double TestDouble { get; set; }

    [Tab("Primitives")] public string TestString { get; set; }

    [Tab("Primitives")] public TestEnum TestEnum { get; set; }

    [Tab("Primitives")] [Range(0, 10)] public float TestRange { get; set; }

    #endregion

    #region Layouting

    [Tab("Layouting")]
    [BoxGroup("Box Group 1")]
    public string TestBoxGroup1 { get; set; }

    [Tab("Layouting")]
    [BoxGroup("Box Group 1")]
    public string TestBoxGroup2 { get; set; }

    [Tab("Layouting")]
    [PropertyOrder(1000)]
    public string TestPropertyOrder4 { get; set; }

    [Tab("Layouting")] [PropertyOrder] public string TestPropertyOrder2 { get; set; }

    [Tab("Layouting")] [PropertyOrder] public string TestPropertyOrder3 { get; set; }

    [Tab("Layouting")] [PropertyOrder(-1)] public string TestPropertyOrder1 { get; set; }

    [Tab("Layouting")] [LabelSize(0.3f)] public string TestLabelSize { get; set; }

    [Tab("Layouting")]
    [BoxGroup("HorizontalGroup1",
        LayoutFlags = LayoutFlags.NotFoldable | LayoutFlags.NoBackground | LayoutFlags.NoLabel,
        Orientation = Orientation.Horizontal)]
    public int TestHVal1 { get; set; }

    [Tab("Layouting")]
    [BoxGroup("HorizontalGroup1")]
    public int TestHVal2 { get; set; }

    [Tab("Layouting")]
    [BoxGroup("HorizontalGroup1")]
    public int TestHVal3 { get; set; }

    [Tab("Layouting")] [Space(30)] public int TestSpace { get; set; }

    #endregion

    #region Complex Types

    [Tab("ComplexTypes")] public VeryComplexType TestInline { get; set; }

    [Tab("ComplexTypes")] public TestingInterface TestInterface { get; set; } = new InterfaceType2();

    [Tab("ComplexTypes")] public List<int> TestList { get; set; }

    [Tab("ComplexTypes")] public int[] TestArray { get; set; }

    [Tab("ComplexTypes")] public List<List<int>> TestListList { get; set; }

    [Tab("ComplexTypes")] public List<VeryComplexType> TestComplexList { get; set; }

    [Tab("ComplexTypes")] public List<TestingInterface> TestPolymorphicList { get; set; }

    #endregion

    #region Other

    [Tab("Other")] public string TestDefaultValue { get; set; } = "Default";

    [Tab("Other")] public string TestReadOnly { get; } = "ReadOnly";

    [Tab("Other")] [Description("This is a property with a description")]
    public string TestDescription;

    [Tab("Other")]
    [Description("This should count up every second")]
    [ReadOnly(true)]
    public int TestUpdates
    {
        get;
        set
        {
            field = value;
            Tick?.Invoke();
        }
    }

    [Tab("Other")]
    [HideInInspector]
    [Description("This property should not be visible")]
    public string TestHidden { get; set; }

    [Tab("Other")]
    [PathPicker(FileDialog.FileModeEnum.OpenFile, ["*.txt"])]
    public string TestFilePath { get; set; }

    [Tab("Other")]
    [PathPicker(FileDialog.FileModeEnum.OpenDir)]
    public string TestDirPath { get; set; }

    [Tab("Other")]
    [Label("Testing Label")]
    [Space(10)]
    [Line]
    public string TestLabel { get; set; }

    [Tab("Other")] [Slider] [Range(0, 1)] public float TestSlider { get; set; } = 0.5f;

    [Tab("Other")] [Slider] [Range(0, 11)] public int IntSlider { get; set; } = 1;
    
    [Tab("Other")] [Suffix("m")] public int TestSuffix { get; set; } = 1;

    #endregion

    public event Action? Tick;
}

public class VeryComplexType
{
    public string TestReadonlyString { get; }
    public int TestInt { get; set; } = 7;
}

public class InterfaceType1 : TestingInterface
{
    public string TestSInterface1 { get; }
}

public class InterfaceType2 : TestingInterface
{
    public string TestSInterface2 { get; }
}

public interface TestingInterface
{
}

public enum TestEnum
{
    Element1,
    Element2,
    Element3
}