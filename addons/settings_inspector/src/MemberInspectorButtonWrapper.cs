using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using LgkProductions.Inspector;
using SettingInspector.addons.settings_inspector.src.InspectorCollections;
using SettingInspector.addons.settings_inspector.src.Inspectors;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorButtonWrapper : Control
{
	[Export] private Label _nameLabel;
	[Export] private Button _cancelButton;
	[Export] private Button _confirmButton;
	[Export] private Node _inspectorContainer;

	private TaskCompletionSource? _tcs;

	public override void _Ready()
	{
		_cancelButton.Pressed += () => _tcs?.SetCanceled();
		_confirmButton.Pressed += () => _tcs?.SetResult();
	}

	public async Task<T> SetInspector<T>(T instance, bool readOnly = false)
	{
		if (_tcs != null)
		{
			GD.PrintErr("Inspector is already open");
			return instance;
		}

        if (instance == null)
        {
            GD.PrintErr("Instance is null");
            return instance;
        }
		
		_tcs = new TaskCompletionSource();
		_nameLabel.Text = typeof(T).Name;

		var memberInspector = MemberInspectorHandler.Instance.GetInputScene(typeof(T)).Instantiate<MemberInspector>();
		_inspectorContainer.AddChild(memberInspector);
		memberInspector.SetInstance(instance, new MemberUiInfo() {AllowTabs = true, Scrollable = true, IsLabelHidden = true, IsBackgroundHidden = true});

		try
		{
			await _tcs.Task;
			
			if (memberInspector.TryRetrieveMember(out var value))
				return (T)value;
			return instance;
		}
		finally
		{
			_tcs = null;
		}
	}
}
