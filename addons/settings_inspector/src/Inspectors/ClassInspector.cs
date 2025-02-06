using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using LgkProductions.Inspector;
using SettingInspector.addons.settings_inspector.src.InspectorCollections;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class ClassInspector : MemberInspector
{
	[Export] private PackedScene _memberCollectionScene;
	[Export] private PackedScene _memberTabCollectionScene;
	[Export] private Node _memberParent;
	[Export] private Button _unattachButton;
	[Export] private Button _loadButton;
	[Export] private Button _saveButton;
	[Export] private Control _lineContainer;
	
	private object? _instance;
	private Node? _memberCollectionNode;
	private IMemberInspectorCollection? MemberInspectorCollection => (IMemberInspectorCollection)_memberCollectionNode;
	
	private FileDialog _fileDialog = FileDialogHandler.CreateNative();
	
	private static readonly JsonSerializerOptions SerializerOptions = new()
	{
		WriteIndented = true
	};
	private static readonly JsonSerializerOptions DeserializerOptions = new()
	{
		PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
	};


	private static PollingTickProvider TickProvider = new(1);

	public override void _EnterTree()
	{
		base._EnterTree();
		_unattachButton.Pressed += UnattachPressed;
		_loadButton.Pressed += LoadPressed;
		_saveButton.Pressed += SavePressed;
		if (_fileDialog.GetParent() == null)
			AddChild(_fileDialog);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_unattachButton.Pressed -= UnattachPressed;
		_loadButton.Pressed -= LoadPressed;
		_saveButton.Pressed -= SavePressed;
	}

	private void UnattachPressed()
	{
		if (_instance == null) return;
		MemberInspectorHandler.Instance.OpenClassInspector(_instance, true, !Editable);
	}

	private void SavePressed()
	{
		_fileDialog.FileMode = FileDialog.FileModeEnum.SaveFile;
		_fileDialog.Filters = ["*.json"];
		
		if (!FileDialogHandler.Popup(_fileDialog, out string filePath)) return;
		
		if (ValueType == null) return;
		
		try
		{
			if (!TryRetrieveMember(out var value)) return;
			var json = JsonSerializer.Serialize(value, ValueType, SerializerOptions);
			File.WriteAllText(filePath, json);
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}
	}

	private void LoadPressed()
	{
		_fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
		_fileDialog.Filters = ["*.json"];

		if (!FileDialogHandler.Popup(_fileDialog, out string filePath)) return;

		if (!File.Exists(filePath))
		{
			GD.PrintErr("File not found: " + filePath);
			return;
		}
		
		var json = File.ReadAllText(filePath);
		
		if (ValueType == null) return;
		try
		{
			var value = JsonSerializer.Deserialize(json, ValueType, DeserializerOptions);
			if (value is not null)
				SetInstance(value, MemberUiInfo);
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}
	}
	

	protected override void SetValue(object classInstance)
	{
		base.SetValue(classInstance);
		_instance = classInstance;
		var inspector = Inspector.Attach(_instance, TickProvider);
		_memberCollectionNode = (MemberUiInfo.AllowTabs ? _memberTabCollectionScene : _memberCollectionScene).Instantiate();
		_memberParent.AddChild(_memberCollectionNode);
		MemberInspectorCollection!.SetMemberInspector(inspector);
		MemberInspectorCollection.SetScrollable(MemberUiInfo.Scrollable);
		MemberInspectorCollection.ValueChanged += OnValueChanged;
	}

	public sealed class PollingTickProvider : ITickProvider
	{
		public event Action? Tick;

		public PollingTickProvider(float pollingRate)
		{
			var timer = new System.Timers.Timer(1/pollingRate);
			timer.Elapsed += (_, __) => Callable.From(() => Tick?.Invoke()).CallDeferred();
			timer.Start();
		}
	}

	protected override void Clear()
	{
		base.Clear();
		_instance = null;
		MemberInspectorCollection?.Remove();
		_memberCollectionNode = null;
	}

	private void ChildValueChanged()
	{
		OnValueChanged();
	}

	protected override object? GetValue()
	{
		//Write values back
		MemberInspectorCollection?.WriteBack();
		return _instance;
	}

	protected override void SetMemberUiInfo(MemberUiInfo memberUiInfo)
	{
		base.SetMemberUiInfo(memberUiInfo);
		_loadButton.Visible = !memberUiInfo.HideButtons;
		_unattachButton.Visible = !memberUiInfo.HideButtons;
		_saveButton.Visible = !memberUiInfo.HideButtons;
		_lineContainer.Visible = !memberUiInfo.IsLabelHidden;
	}

	public override void SetEditable(bool editable)
	{
		base.SetEditable(editable);
		MemberInspectorCollection?.SetEditable(editable);
	}
}
