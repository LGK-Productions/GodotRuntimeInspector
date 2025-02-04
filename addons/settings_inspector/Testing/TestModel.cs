using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LgkProductions.Inspector.Attributes;

namespace SettingInspector.addons.settings_inspector.Testing;

internal class TestModel
{
    public bool TestBool { get; set; }
    
    [Range(0, 10)]
    public int TestRange {get; set;}

    public VeryComplexType TestVeryComplex { get; set; } = new();

    [BoxGroup("Group1")]
    [Range(0, 10)]
    public float TestFloat { get; set; } = 3.2f;

    [TabGroup("Tab2")]
    public string TestTab { get; set; } = "new tab";

    [TabGroup("Tab2")] public List<int> TestList { get; set; } = [0, 1, 2];
    
    [Range(0, 10)]
    public float TestDouble { get; set; } = 20f;

    public TestEnum TestEnum { get; set; } = Testing.TestEnum.Element2;

    [Display(Name = "Name")]
    [DisplayName("Name2")]
    public string TestName { get; set; } = "Name should be \"Name2\"";

    [Display(Description = "Description")]
    [Description("Description2")]
    public string TestDescription { get; set; } = "Hover for description";
    
    [Editable(allowEdit: true)]
    [ReadOnly(isReadOnly: true)]
    public string TestReadOnly { get; set; }

    [HideInInspector]
    [Browsable(false)]
    public int TestHidden { get; set; }

    [ShowInInspector]
    [Browsable(true)]
    private int TestShown { get; set; }

    [PropertyOrder(1_000_000)]
    [Display(Order = 1)]
    public string TestOrderFar { get; set; }

    [PropertyOrder]
    public string TestOrder1 { get; set; }

    [PropertyOrder]
    public string TestOrder2 { get; set; }

    [PropertyOrder(-1)]
    [Display(Order = -1)]
    public int TestOrder0 { get; set; }

    [BoxGroup("Group1")]
    [Display(GroupName = "Group1")]
    [Category("Group1")]
    public int TestGroup1 { get; set; }

    public int TestDefaultValue { get; set; } = 42;
}

public class VeryComplexType
{
    public string TestString { get; set; }
    public int TestInt { get; set; } = 7;
}

public enum TestEnum
{
    Element1,
    Element2,
    Element3
}