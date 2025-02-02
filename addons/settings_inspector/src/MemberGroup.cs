using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberGroup : Control
{
    [Export] private Label _groupNameLabel;
    [Export] private Node _memberParent;


    public void SetGroup(string groupName)
    {
        _groupNameLabel.Text = groupName;
    }

    public void AddMember(MemberInspector memberInspector)
    {
        _memberParent.AddChild(memberInspector);
    }
}