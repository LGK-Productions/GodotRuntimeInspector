using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Timers;
using LgkProductions.Inspector.Attributes;
using SettingInspector.addons.settings_inspector.src.Attributes;
using SettingInspector.addons.settings_inspector.src.InspectorCollections;

namespace SettingInspector.addons.settings_inspector.Testing;

internal class TestModel
{
    public TestModel()
    {
        var timer = new Timer(1000);
        timer.Elapsed += (sender, args) => TestUpdates++;
        timer.Start();
    }
    
    #region Primitives

    [TabGroup("Primitives")]
    public bool TestBool { get; set; }
    
    [TabGroup("Primitives")]
    public int TestInt { get; set; }
    
    [TabGroup("Primitives")]
    public float TestFloat { get; set; }
    
    [TabGroup("Primitives")]
    public double TestDouble { get; set; }
    
    [TabGroup("Primitives")]
    public string TestString { get; set; }
    
    [TabGroup("Primitives")]
    public TestEnum TestEnum { get; set; }
    
    [TabGroup("Primitives")]
    [Range(0, 10)]
    public float TestRange { get; set; }

    #endregion

    #region Layouting

    [TabGroup("Layouting")]
    [BoxGroup("Box Group 1")]
    public string TestBoxGroup1 { get; set; }
    
    [TabGroup("Layouting")]
    [BoxGroup("Box Group 1")]
    public string TestBoxGroup2 { get; set; }
    
    [TabGroup("Layouting")]
    [PropertyOrder(1)]
    public string TestPropertyOrder4 { get; set; } 
    
    [TabGroup("Layouting")]
    [PropertyOrder]
    public string TestPropertyOrder2 { get; set; } 
    
    [TabGroup("Layouting")]
    [PropertyOrder]
    public string TestPropertyOrder3 { get; set; }
    
    [TabGroup("Layouting")]
    [PropertyOrder(-1)]
    public string TestPropertyOrder1 { get; set; } 
    
    [TabGroup("Layouting")]
    [LabelSize(0.3f)]
    public string TestLabelSize { get; set; }
    
    [TabGroup("Layouting")]
    [HorizontalGroup("HorizontalGroup1")]
    public int TestHVal1 { get; set; }
    
    [TabGroup("Layouting")]
    [HorizontalGroup("HorizontalGroup1")]
    public int TestHVal2 { get; set; }
    
    [TabGroup("Layouting")]
    [HorizontalGroup("HorizontalGroup1")]
    public int TestHVal3 { get; set; }

    #endregion

    #region Complex Types

    [TabGroup("ComplexTypes")]
    public VeryComplexType TestInline { get; set; }
    
    [TabGroup("ComplexTypes")]
    public List<int> TestList { get; set; }
    
    [TabGroup("ComplexTypes")]
    public List<List<int>> TestListList { get; set; }
    
    [TabGroup("ComplexTypes")]
    public List<VeryComplexType> TestComplexList { get; set; }

    #endregion

    #region Other

    [TabGroup("Other")]
    public string TestDefaultValue = string.Empty;
    
    [TabGroup("Other")]
    public string TestReadOnly { get; } = "ReadOnly";
    
    [TabGroup("Other")]
    [Description("This is a property with a description")]
    public string TestDescription;
    
    [TabGroup("Other")]
    [Description("This should count up every second")]
    [ReadOnly(isReadOnly: true)]
    public int TestUpdates { get; set; }
    
    [TabGroup("Other")]
    [HideInInspector]
    [Description("This property should not be visible")]
    public string TestHidden { get; set; }
    
    [TabGroup("Other")]
    [FilePath]
    public string TestFilePath { get; set; }

    #endregion
}

public class VeryComplexType
{
    public string TestReadonlyString { get;}
    public int TestInt { get; set; } = 7;
}

public enum TestEnum
{
    Element1,
    Element2,
    Element3
}