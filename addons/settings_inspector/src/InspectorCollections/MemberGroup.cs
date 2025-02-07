using Godot;

namespace SettingInspector.addons.settings_inspector.src.InspectorCollections;

public partial class MemberGroup : Control
{
	[Export] private Label _groupNameLabel;
	[Export] private Control _background;
	[Export] private Control _lineContainer;
	[Export] private Node _memberParent;

	public void SetGroup(string groupName)
	{
		_groupNameLabel.Text = groupName;
	}

	public void AddMember(MemberInspector memberInspector)
	{
		_memberParent.AddChild(memberInspector);
	}

	public void SetStyleMode(GroupStyleMode mode)
	{
		switch (mode)
		{
			case GroupStyleMode.HasTitle:
				_background.Visible = true;
				_lineContainer.Visible = true;
				_groupNameLabel.Visible = true;
				break;
			case GroupStyleMode.NoTitle:
				_background.Visible = false;
				_lineContainer.Visible = false;
				_groupNameLabel.Visible = false;
				break;
		}
	}
}

public enum GroupStyleMode
{
	HasTitle,
	NoTitle
}
