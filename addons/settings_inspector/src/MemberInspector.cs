using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.InputControllers;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspector : Node
{
	[Export] private Label _label;
	[Export] private Control _inputContainer;
	[Export] private PackedScene _lineInputScene;

	private InspectorElement? _inspectorMember;
	private IMemberInput? _memberInput;

	public void SetMember(InspectorElement iElement)
	{
		_label.Text = iElement.MemberInfo.Name;
		_label.TooltipText = iElement.MemberInfo.Description;
		
		var lineInput = _lineInputScene.Instantiate<LineInput>();
		_inputContainer.AddChild(lineInput);
		lineInput.SetValue(iElement.Value);
		
		_inspectorMember = iElement;
		_memberInput = lineInput;
	}

	public bool TryRetrieveMember<T>(out T? result)
	{
		result = default;
		if (_inspectorMember == null)
		{
			GD.PrintErr("Could not retrieve member, due to no member being set");
			return false;
		}

		return _memberInput.TryGetValue<T>(out result);
	}
}
