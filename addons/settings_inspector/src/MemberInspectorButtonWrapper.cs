using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorButtonWrapper : Control
{
	[Export] private Label _nameLabel;
	[Export] private Button _cancelButton;
	[Export] private Button _confirmButton;
	[Export] private Node _inspectorContainer;
	[Export] private PackedScene _memberCollectionScene;

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
		
		_tcs = new TaskCompletionSource();
		_nameLabel.Text = typeof(T).Name;

		var inspector = Inspector.Attach(instance, ClassInspector.TickProvider);
		var memberCollection = _memberCollectionScene.Instantiate<MemberInspectorCollection>();
		_inspectorContainer.AddChild(memberCollection);
		memberCollection.SetMemberInspector(inspector);
        memberCollection.SetEditable(!readOnly);

		try
		{
			await _tcs.Task;
			
			memberCollection.WriteBack();
			return instance;
		}
		finally
		{
			_tcs = null;
		}
	}
}
