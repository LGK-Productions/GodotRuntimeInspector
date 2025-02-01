using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LgkProductions.Inspector.Attributes;

namespace SettingInspector.addons.settings_inspector.Testing;

internal class TestModel
{
    [Display(Name = "Name")]
    [DisplayName("Name2")]
    public int TestName { get; set; }

    [Display(Description = "Description")]
    [Description("Description2")]
    public int TestDescription { get; set; }

    [Editable(allowEdit: true)]
    [ReadOnly(isReadOnly: true)]
    public int TestReadOnly { get; set; }

    [HideInInspector]
    [Browsable(false)]
    public int TestHidden { get; set; }

    [ShowInInspector]
    [Browsable(true)]
    private int TestShown { get; set; }

    [PropertyOrder(1_000_000)]
    [Display(Order = 1)]
    public int TestOrderFar { get; set; }

    [PropertyOrder]
    public int TestOrder1 { get; set; }

    [PropertyOrder]
    public int TestOrder2 { get; set; }

    [PropertyOrder(-1)]
    [Display(Order = -1)]
    public int TestOrder0 { get; set; }

    [BoxGroup("Group1")]
    [Display(GroupName = "Group1")]
    [Category("Group1")]
    public int TestGroup1 { get; set; }

    public int TestDefaultValue { get; set; } = 42;
}