using Godot;
using LgkProductions.Inspector;
using SettingInspector.Util;
using Orientation = LgkProductions.Inspector.Orientation;

namespace SettingInspector.Inspectors.Collections;

public partial class MemberGroup : Control
{
    [Export] private Control? _background;
    [Export] private Button? _expandButton;
    [Export] private Label? _groupNameLabel;

    private BoxContainer? _memberParent;
    [Export] private HBoxContainer? _memberParentHorizontal;
    [Export] private VBoxContainer? _memberParentVertical;

    public override void _EnterTree()
    {
        base._EnterTree();
        _expandButton!.Toggled += ExpandButtonToggled;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _expandButton!.Toggled -= ExpandButtonToggled;
    }

    private void ExpandButtonToggled(bool on)
    {
        _memberParent?.Visible = on;
    }

    public void SetGroup(GroupLayout groupLayout)
    {
        _memberParent = groupLayout.Orientation switch
        {
            Orientation.Vertical => _memberParentVertical,
            Orientation.Horizontal => _memberParentHorizontal,
            _ => _memberParent
        };

        _expandButton!.Visible = !groupLayout.LayoutFlags.IsSet(LayoutFlags.NotFoldable);
        ExpandButtonToggled(true);

        _groupNameLabel!.Text = groupLayout.Title;
        _groupNameLabel.Visible = !groupLayout.LayoutFlags.IsSet(LayoutFlags.NoLabel);
        _background!.Visible = !groupLayout.LayoutFlags.IsSet(LayoutFlags.NoBackground);
        _memberParent!.Visible = groupLayout.LayoutFlags.IsSet(LayoutFlags.ExpandedInitially);
    }

    public void AddMember(MemberWrapper memberWrapper)
    {
        _memberParent!.AddChild(memberWrapper);
    }
}