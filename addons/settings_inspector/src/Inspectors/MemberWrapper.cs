using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class MemberWrapper : MarginContainer
{
	[Export] private PackedScene? _defaultInspectorScene;
	[Export] private Array<PackedScene?> _inspectorScenes = new();

	[Export] private Control? _memberContainer;
	
	public MemberInspector? MemberInspector { get; private set; }
	
	public MemberInspector SetMemberType(Type type)
	{
		MemberInspector = GetInputScene(type)!.Instantiate<MemberInspector>();
		_memberContainer?.AddChild(MemberInspector);
        MemberInspector.SetWrapper(this);
		return MemberInspector;
	}

	public void SetMargin(int top, int bottom, int left, int right)
	{
		_memberContainer.AddThemeConstantOverride("margin_top", top);
		_memberContainer.AddThemeConstantOverride("margin_bottom", bottom);
		_memberContainer.AddThemeConstantOverride("margin_left", left);
		_memberContainer.AddThemeConstantOverride("margin_right", right);
	}
	
	private PackedScene? GetInputScene(Type inputType)
	{
		if (inputType == typeof(bool))
		{
			if (_inspectorScenes.Count > 0)
				return _inspectorScenes[0];
		}
		if (inputType == typeof(int))
		{
			if (_inspectorScenes.Count > 1)
				return _inspectorScenes[1];
		}
		if (inputType == typeof(float))
		{
			if (_inspectorScenes.Count > 2)
				return _inspectorScenes[2];
		}
		if (inputType == typeof(double))
		{
			if (_inspectorScenes.Count > 3)
				return _inspectorScenes[3];
		}
		if (inputType.IsEnum)
		{
			if (_inspectorScenes.Count > 4)
				return _inspectorScenes[4];
		}

		if (inputType.IsGenericType && inputType.GetGenericTypeDefinition() == typeof(List<>))
		{
			if (_inspectorScenes.Count > 6)
				return _inspectorScenes[6];
		}
		if (!inputType.IsPrimitive && inputType != typeof(string))
		{
			if (_inspectorScenes.Count > 5)
				return _inspectorScenes[5];
		}

		return _defaultInspectorScene;
	}
}
