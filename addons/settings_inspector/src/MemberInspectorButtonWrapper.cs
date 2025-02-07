using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Godot;
using SettingInspector.addons.settings_inspector.src.Inspectors;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorButtonWrapper : Control
{
	[Export] private Button _cancelButton;
	[Export] private Button _confirmButton;
	[Export] private Node _inspectorContainer;

	private static readonly MemberUiInfo MemberUiInfo = new ()
	{
		AllowTabs = true, Scrollable = true, IsLabelHidden = false, IsBackgroundHidden = true, IsExpanded = true
	};
	
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

		var inspector = MemberInspectorHandler.Instance.GetInputScene(typeof(T)).Instantiate<MemberInspector>();
		_inspectorContainer.AddChild(inspector);

		inspector.SetInstance(instance, MemberUiInfo);
		try
		{
			await _tcs.Task;

			if (!inspector.TryRetrieveMember(out var value))
				throw new NullReferenceException();
			return (T)value;
		}
		finally
		{
			inspector.Remove();
			_tcs = null;
		}
	}
}
