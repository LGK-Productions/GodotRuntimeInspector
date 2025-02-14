using Godot;
using LgkProductions.Inspector;
using Orientation = LgkProductions.Inspector.Orientation;

namespace SettingInspector.addons.settings_inspector.src.InspectorCollections;

public partial class MemberGroup : Control
{
	[Export] private Label _groupNameLabel;
	[Export] private Control _background;
	[Export] private Control _lineContainer;
	[Export] private HBoxContainer _memberParentHorizontal;
	[Export] private VBoxContainer _memberParentVertical;

	private BoxContainer _memberParent;
	public void SetGroup(GroupLayout groupLayout)
	{
		_memberParent = groupLayout.Orientation switch
		{
			Orientation.Vertical => _memberParentVertical,
			Orientation.Horizontal => _memberParentHorizontal,
			_ => _memberParent
		};

		_groupNameLabel.Text = groupLayout.Title;
		_groupNameLabel.Visible = groupLayout.HasFrame;
		_background.Visible = groupLayout.HasFrame;
	}

	public void AddMember(MemberInspector memberInspector)
	{
		_memberParent.AddChild(memberInspector);
	}
}

public enum GroupStyleMode
{
	HasTitle,
	NoTitle
}
