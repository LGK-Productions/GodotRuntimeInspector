using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
	[Export] private Control _memberParent;
	[Export] private Button? _unattachButton;
	[Export] private Button? _loadButton;
	[Export] private Button? _saveButton;
	[Export] private ToggleButton? _expandButton;
	[Export] private OptionButton _typeChooser;

	private const bool AllowUnattach = false;

	private object? _instance;
	private Type[]? _assignables;
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

	protected override void OnInitialize()
	{
		_unattachButton.Pressed += UnattachPressed;
		_loadButton.Pressed += LoadPressed;
		_saveButton.Pressed += SavePressed;
		_expandButton.Toggled += ExpandButtonToggled;
		_typeChooser.IndexSelected += TypeIndexSelected;
		if (_fileDialog.GetParent() == null)
			AddChild(_fileDialog);
	}

	protected override void OnRemove()
	{
		_unattachButton.Pressed -= UnattachPressed;
		_loadButton.Pressed -= LoadPressed;
		_saveButton.Pressed -= SavePressed;
		_expandButton.Toggled -= ExpandButtonToggled;
		_typeChooser.IndexSelected += TypeIndexSelected;
	}

	protected override void SetValue(object classInstance)
	{
		base.SetValue(classInstance);
		_instance = classInstance;
		var inspector = Inspector.Attach(_instance, TickProvider);
		bool serializable = classInstance.GetType().GetCustomAttributes<SerializableAttribute>().Any();

		_expandButton?.SetVisible(_expandButton.Visible &&
								  (inspector.Elements.Count > 0 || MemberUiInfo.parentType != null));
		_saveButton?.SetVisible(_saveButton.Visible && serializable);
		_loadButton?.SetVisible(_loadButton.Visible && serializable);
		_unattachButton?.SetVisible(_unattachButton.Visible && AllowUnattach);
		if (_assignables != null)
			_typeChooser.SetSelectedIndex(Array.IndexOf(_assignables, classInstance.GetType()));

		_memberCollectionNode =
			(MemberUiInfo.AllowTabs ? _memberTabCollectionScene : _memberCollectionScene).Instantiate();
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
			var timer = new System.Timers.Timer(1 / pollingRate);
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

	protected override void SetLayoutFlags(LayoutFlags flags)
	{
		base.SetLayoutFlags(flags);

		var expanded = flags.IsSet(LayoutFlags.ExpandedInitially);
		_expandButton?.SetPressed(expanded);
		_expandButton?.SetVisible(!flags.IsSet(LayoutFlags.NotFoldable));

		var elementsVisible = !flags.IsSet(LayoutFlags.NoElements);
		_loadButton?.SetVisible(elementsVisible);
		_saveButton?.SetVisible(elementsVisible);
		_unattachButton?.SetVisible(elementsVisible);
	}

	protected override void SetMemberUiInfo(MemberUiInfo memberUiInfo)
	{
		base.SetMemberUiInfo(memberUiInfo);
		if (MemberUiInfo.parentType != null)
			SetParentType(MemberUiInfo.parentType);
		else
			_assignables = null;
	}

	private void SetParentType(Type parentType)
	{
		_assignables = Util.GetAssignableTypes(parentType).ToArray();
		_typeChooser.SetOptions(_assignables.Select(t => t.Name));
	}

	public override void SetEditable(bool editable)
	{
		base.SetEditable(editable);
		MemberInspectorCollection?.SetEditable(editable);
	}

	#region Buttons

	private void ExpandButtonToggled(bool on)
	{
		_memberParent.Visible = on;
		_typeChooser.Visible = MemberUiInfo.parentType != null && on;
	}

	private void UnattachPressed()
	{
		if (_instance == null) return;
		MemberInspectorHandler.Instance.OpenClassInspector(_instance);
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

	private void TypeIndexSelected(int index)
	{
		if (_assignables == null || index < 0 || index >= _assignables.Length) return;
		if (!Util.TryCreateInstance(_assignables[index], out var instance)) return;
		SetInstance(instance, MemberUiInfo,
			LayoutFlags.Set(LayoutFlags.ExpandedInitially, _expandButton.ButtonPressed));
	}

	#endregion
}
