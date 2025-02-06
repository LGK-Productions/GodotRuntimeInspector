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
	[Export] private Label _nameLabel;
	[Export] private Button _cancelButton;
	[Export] private Button _confirmButton;
	[Export] private Button _saveButton;
	[Export] private Button _loadButton;
	[Export] private Node _inspectorContainer;
	
	private static readonly JsonSerializerOptions SerializerOptions = new()
	{
		WriteIndented = true
	};
	private static readonly JsonSerializerOptions DeserializerOptions = new()
	{
		PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
	};

	private static readonly MemberUiInfo MemberUiInfo = new ()
	{
		AllowTabs = true, Scrollable = true, IsLabelHidden = true, IsBackgroundHidden = true
	};
	
	private FileDialog _fileDialog = FileDialogFactory.CreateNativeJson();

	private TaskCompletionSource? _tcs;
	private MemberInspector? _inspector;

	public override void _Ready()
	{
		_cancelButton.Pressed += () => _tcs?.SetCanceled();
		_confirmButton.Pressed += () => _tcs?.SetResult();
		_loadButton.Pressed += LoadButtonPressed;
		_saveButton.Pressed += SaveButtonPressed;
		AddChild(_fileDialog);
	}

	private void SaveButtonPressed()
	{
		_fileDialog.FileMode = FileDialog.FileModeEnum.SaveFile;
		
		_fileDialog.FileSelected += FileDialogConfirmed;
		_fileDialog.PopupCentered();
		_fileDialog.FileSelected -= FileDialogConfirmed;
		

		void FileDialogConfirmed(string path)
		{
			SaveJson(path);
		}
	}
	private void LoadButtonPressed()
	{
		_fileDialog.FileSelected += FileDialogConfirmed;
		
		_fileDialog.PopupCentered();
		
		_fileDialog.FileSelected -= FileDialogConfirmed;


		
		void FileDialogConfirmed(string filePath)
		{
			if (!File.Exists(filePath))
			{
				GD.PrintErr("File not found: " + filePath);
				return;
			}
			LoadJson(File.ReadAllText(filePath));
		}
	}

	private void LoadJson(string json)
	{
		if (_inspector?.ValueType == null) return;
		try
		{
			var value = JsonSerializer.Deserialize(json, _inspector.ValueType, DeserializerOptions);
			if (value is not null)
				_inspector.SetInstance(value, MemberUiInfo);
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}
	}

	private void SaveJson(string path)
	{
		if (_inspector?.ValueType == null) return;
		
		try
		{
			if (!_inspector.TryRetrieveMember(out var value)) return;
			var json = JsonSerializer.Serialize(value, _inspector.ValueType, SerializerOptions);
			File.WriteAllText(path, json);
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}
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

		_inspector = MemberInspectorHandler.Instance.GetInputScene(typeof(T)).Instantiate<MemberInspector>();
		_inspectorContainer.AddChild(_inspector);

		_inspector.SetInstance(instance, MemberUiInfo);
		try
		{
			await _tcs.Task;

			if (!_inspector.TryRetrieveMember(out var value))
				throw new NullReferenceException();
			return (T)value;
		}
		finally
		{
			_inspector.Remove();
			_inspector = null;
			_tcs = null;
		}
	}
}
