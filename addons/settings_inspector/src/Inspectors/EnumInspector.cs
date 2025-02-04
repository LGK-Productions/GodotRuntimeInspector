using System;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.InputControllers;

public partial class EnumInspector : MemberInspector
{
	[Export] private OptionButton _optionButton;
	
	private readonly List<string> _enumLabels = new();
    
	public override void _EnterTree()
	{
        base._EnterTree();
		_optionButton.ItemSelected += OnItemSelected;
	}

	public override void _ExitTree()
	{
        base._ExitTree();
		_optionButton.ItemSelected -= OnItemSelected;
	}

	private void OnItemSelected(long item)
    {
        OnValueChanged();
    }

	protected override void SetValue(object? value)
	{
        if (value == null || Enum.IsDefined(value.GetType(), value) == false || ValueType == null) return;
        var name = Enum.GetName(ValueType, value);
        if (name == null) return;
		_optionButton.Selected = _enumLabels.IndexOf(name);
	}

	protected override object? GetValue()
    {
        return ValueType == null ? null : Enum.Parse(ValueType, _enumLabels[_optionButton.Selected]);
    }

	public override void SetEditable(bool editable)
	{
		_optionButton.Disabled = !editable;
	}

	protected override void OnSetMetaData(MetaDataMember member)
	{
		foreach (var label in Enum.GetNames(member.Type))
		{
			_optionButton.AddItem(label);
			_enumLabels.Add(label);
		}
	}

}
