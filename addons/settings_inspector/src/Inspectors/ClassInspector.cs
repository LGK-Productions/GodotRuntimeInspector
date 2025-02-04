using System;
using Godot;
using LgkProductions.Inspector;
using SettingInspector.addons.settings_inspector.src.InspectorCollections;

namespace SettingInspector.addons.settings_inspector.src;

public partial class ClassInspector : MemberInspector
{
	[Export] private PackedScene _memberCollectionScene;
	[Export] private Node _memberParent;
	[Export] private Button _unattachButton;
	
	private object? _instance;
	private IMemberInspectorCollection? _memberInspectorCollection;

	public override void _EnterTree()
	{
		base._EnterTree();
		_unattachButton.Pressed += () =>
		{
			if (_instance == null) return;
			MemberInspectorHandler.Instance.OpenClassInspector(_instance, true, InspectorElement.MemberInfo.IsReadOnly);
		};
	}
	
	public static PollingTickProvider TickProvider = new(1);

	protected override void SetValue(object? classInstance)
	{
		if (classInstance == null)
		{
			GD.Print("classInstance is null. Trying to create new instance.");
			try
			{
				classInstance = Activator.CreateInstance(InspectorElement.MemberInfo.Type);
			}
			catch (Exception e)
			{
				GD.PrintErr(e);
				return;
			}
		}
		_instance = classInstance;
		var inspector = Inspector.Attach(classInstance, TickProvider);
		var memberCollectionNode = _memberCollectionScene.Instantiate();
		_memberParent.AddChild(memberCollectionNode);
		_memberInspectorCollection = (IMemberInspectorCollection)memberCollectionNode;
		_memberInspectorCollection.SetMemberInspector(inspector);
		_memberInspectorCollection.SetEditable(!InspectorElement.MemberInfo.IsReadOnly);
		_memberInspectorCollection.ValueChanged += OnValueChanged;
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

	private void ChildValueChanged()
	{
		OnValueChanged();
	}

	protected override object? GetValue()
	{
		//Write values back
		_memberInspectorCollection?.WriteBack();
		return _instance;
	}

	public override void SetEditable(bool editable)
	{
		_memberInspectorCollection?.SetEditable(editable);
	}
}
