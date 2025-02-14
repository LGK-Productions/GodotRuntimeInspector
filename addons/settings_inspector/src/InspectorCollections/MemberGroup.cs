using Godot;
using LgkProductions.Inspector;
using Orientation = LgkProductions.Inspector.Orientation;

namespace SettingInspector.addons.settings_inspector.src.InspectorCollections;

public partial class MemberGroup : Control
{
	[Export] private Label _groupNameLabel;
	[Export] private Control _background;
	[Export] private HBoxContainer _memberParentHorizontal;
	[Export] private VBoxContainer _memberParentVertical;
	[Export] private Button _expandButton;

	private BoxContainer? _memberParent;

    public override void _EnterTree()
    {
        base._EnterTree();
        _expandButton.Toggled += ExpandButtonToggled;
    }

    public override void _ExitTree()
	{
		base._ExitTree();
		_expandButton.Toggled -= ExpandButtonToggled;
	}

	private void ExpandButtonToggled(bool on)
	{
        if (_memberParent != null)
		    _memberParent.Visible = on;
	}

	public void SetGroup(GroupLayout groupLayout)
	{
		_memberParent = groupLayout.Orientation switch
		{
			Orientation.Vertical => _memberParentVertical,
			Orientation.Horizontal => _memberParentHorizontal,
			_ => _memberParent
		};
        
		_expandButton.Visible = groupLayout.IsFoldable && groupLayout.HasFrame;
        ExpandButtonToggled(true);

		_groupNameLabel.Text = groupLayout.Title;
		_groupNameLabel.Visible = groupLayout.HasFrame;
		_background.Visible = groupLayout.HasFrame;
	}

	public void AddMember(MemberInspector memberInspector)
	{
		_memberParent.AddChild(memberInspector);
	}
}
