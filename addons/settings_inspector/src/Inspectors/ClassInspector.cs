using System;
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
	
	private object? _instance;
	private Node? _memberCollectionNode;
    private IMemberInspectorCollection? MemberInspectorCollection => (IMemberInspectorCollection)_memberCollectionNode;

	public override void _EnterTree()
	{
		base._EnterTree();
		_unattachButton.Pressed += OpenMemberAsWindow;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_unattachButton.Pressed -= OpenMemberAsWindow;
	}

	private void OpenMemberAsWindow()
	{
		if (_instance == null) return;
		MemberInspectorHandler.Instance.OpenClassInspector(_instance, true, !Editable);
	}
	
	public static PollingTickProvider TickProvider = new(1);

	protected override void SetValue(object classInstance)
	{
        base.SetValue(classInstance);
        _instance = classInstance;
		var inspector = Inspector.Attach(_instance, TickProvider);
		_memberCollectionNode = (MemberUiInfo.AllowTabs ? _memberTabCollectionScene : _memberCollectionScene).Instantiate();
		_memberParent.AddChild(_memberCollectionNode);
        MemberInspectorCollection.SetMemberInspector(inspector);
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
		MemberInspectorCollection?.Clear();
        _memberCollectionNode?.QueueFree();
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

	public override void SetEditable(bool editable)
	{
		base.SetEditable(editable);
        MemberInspectorCollection?.SetEditable(editable);
	}
}
