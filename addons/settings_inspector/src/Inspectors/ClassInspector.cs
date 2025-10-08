using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using Microsoft.Extensions.Logging;
using SettingInspector.addons.settings_inspector.Inspectors.InspectorCollections;
using SettingInspector.addons.settings_inspector.ValueTree;
using Timer = System.Timers.Timer;

namespace SettingInspector.addons.settings_inspector.Inspectors;

public partial class ClassInspector : MemberInspector
{
    private const bool AllowUnattach = false;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private static readonly JsonSerializerOptions DeserializerOptions = new()
    {
        PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
    };


    private static readonly PollingTickProvider SecondTickProvider = new(1);


    private readonly FileDialogHandler.FileDialogHandle _fileDialogHandle = FileDialogHandler.CreateNative();
    private Type[]? _assignables;
    [Export] private FoldableContainer? _foldableContainer;

    private object? _instance;
    [Export] private Button? _loadButton;
    private Node? _memberCollectionNode;
    [Export] private PackedScene? _memberCollectionScene;
    [Export] private Control? _memberParent;
    [Export] private PackedScene? _memberTabCollectionScene;
    [Export] private Button? _saveButton;
    [Export] private OptionButton? _typeChooser;
    [Export] private Button? _unattachButton;
    private IMemberInspectorCollection? MemberInspectorCollection => (IMemberInspectorCollection?)_memberCollectionNode;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        _unattachButton!.Pressed += UnattachPressed;
        _loadButton!.Pressed += LoadPressed;
        _saveButton!.Pressed += SavePressed;
        _typeChooser!.IndexSelected += TypeIndexSelected;
        if (_fileDialogHandle.FileDialog.GetParent() == null)
            AddChild(_fileDialogHandle.FileDialog);
    }

    protected override void OnRemove()
    {
        base.OnRemove();
        _unattachButton!.Pressed -= UnattachPressed;
        _loadButton!.Pressed -= LoadPressed;
        _saveButton!.Pressed -= SavePressed;
        _typeChooser!.IndexSelected += TypeIndexSelected;
    }

    protected override void SetValueInternal(object classInstance)
    {
        _instance = classInstance;
        var inspector = _instance is ITickProvider provider
            ? Inspector.Attach(_instance, provider)
            : Inspector.Attach(_instance, SecondTickProvider);
        var serializable = classInstance.GetType().GetCustomAttributes<SerializableAttribute>().Any();

        _saveButton?.SetVisible(_saveButton.Visible && serializable);
        _loadButton?.SetVisible(_loadButton.Visible && serializable);
        _unattachButton?.SetVisible(_unattachButton.Visible && AllowUnattach);
        _typeChooser!.Visible = MemberUiInfo.ParentType != null;

        if (_assignables != null)
            _typeChooser.SetSelectedIndex(Array.IndexOf(_assignables, classInstance.GetType()));
        if (Element == null)
            _foldableContainer?.Title = ValueType?.Name;

        _memberCollectionNode =
            (MemberUiInfo.AllowTabs ? _memberTabCollectionScene : _memberCollectionScene)!.Instantiate();
        _memberParent!.AddChild(_memberCollectionNode);
        MemberInspectorCollection!.SetMemberInspector(inspector);
        MemberInspectorCollection.SetScrollable(MemberUiInfo.Scrollable);
        MemberInspectorCollection.ValueChanged += ChildValueChanged;
    }

    protected override void OnSetMetaData(MetaDataMember member)
    {
        base.OnSetMetaData(member);
        _foldableContainer?.Title = member.DisplayName;
        _foldableContainer?.TooltipText = member.Description;
    }

    protected override void Clear()
    {
        base.Clear();
        _instance = null;
        MemberInspectorCollection?.Remove();
        _memberCollectionNode = null;
    }

    private void ChildValueChanged(ValueChangeTree tree)
    {
        OnValueChanged(new ValueChangeTree(this, _instance, tree));
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
        _foldableContainer?.SetFolded(!expanded);
        //_expandButton?.SetVisible(!flags.IsSet(LayoutFlags.NotFoldable));

        var elementsVisible = !flags.IsSet(LayoutFlags.NoElements);
        _loadButton?.SetVisible(elementsVisible);
        _saveButton?.SetVisible(elementsVisible);
        _unattachButton?.SetVisible(elementsVisible);

        if (flags.IsSet(LayoutFlags.NoBackground))
        {
            _foldableContainer?.SetVisible(false);
            _memberParent?.GetParent()?.Reparent(this);
        }

        if (flags.IsSet(LayoutFlags.NoLabel))
        {
            _foldableContainer?.Title = "";
            _foldableContainer?.TooltipText = "";
        }
    }

    protected override void SetMemberUiInfo(MemberUiInfo memberUiInfo)
    {
        base.SetMemberUiInfo(memberUiInfo);
        if (MemberUiInfo.ParentType != null)
            SetParentType(MemberUiInfo.ParentType);
        else
            _assignables = null;
    }

    private void SetParentType(Type parentType)
    {
        _assignables = Util.GetAssignableTypes(parentType).ToArray();
        _typeChooser!.SetOptions(_assignables.Select(t => t.Name));
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        MemberInspectorCollection?.SetEditable(editable);
    }

    public sealed class PollingTickProvider : ITickProvider
    {
        public PollingTickProvider(float pollingRate)
        {
            var timer = new Timer(1 / pollingRate);
            timer.Elapsed += (_, __) => Callable.From(() => Tick?.Invoke()).CallDeferred();
            timer.Start();
        }

        public event Action? Tick;
    }

    #region Buttons

    private void UnattachPressed()
    {
        if (_instance == null) return;
        MemberInspectorHandler.Instance.OpenClassInspectorWindow(_instance);
    }

    private async void SavePressed()
    {
        _fileDialogHandle.FileDialog.FileMode = FileDialog.FileModeEnum.SaveFile;
        _fileDialogHandle.FileDialog.Filters = ["*.json"];

        var filePath = await _fileDialogHandle.WaitForFileSelectedAsync();

        if (ValueType == null) return;

        try
        {
            if (!TryRetrieveMember(out var value)) return;
            var json = JsonSerializer.Serialize(value, ValueType, SerializerOptions);
            File.WriteAllText(filePath, json);
        }
        catch (Exception e)
        {
            MemberInspectorHandler.Logger?.LogError(e, "Failed to save file");
        }
    }

    private async void LoadPressed()
    {
        _fileDialogHandle.FileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        _fileDialogHandle.FileDialog.Filters = ["*.json"];

        var filePath = await _fileDialogHandle.WaitForFileSelectedAsync();

        if (!File.Exists(filePath))
        {
            MemberInspectorHandler.Logger?.LogError("File not found: {filePath}", filePath);
            return;
        }

        var json = File.ReadAllText(filePath);

        if (ValueType == null) return;
        try
        {
            var value = JsonSerializer.Deserialize(json, ValueType, DeserializerOptions);
            if (value is not null)
                SetInstance(value, MemberUiInfo, LayoutFlags);
            OnValueChanged(new ValueChangeTree(this, _instance));
        }
        catch (Exception e)
        {
            MemberInspectorHandler.Logger?.LogError(e, "Failed to save file");
        }
    }

    private void TypeIndexSelected(int index)
    {
        if (_assignables == null || index < 0 || index >= _assignables.Length) return;
        if (!Util.TryCreateInstance(_assignables[index], out var instance) || instance is null) return;
        SetInstance(instance, MemberUiInfo, LayoutFlags | LayoutFlags.ExpandedInitially);
    }

    #endregion
}