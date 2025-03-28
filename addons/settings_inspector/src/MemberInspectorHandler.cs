using System;
using Godot;
using LgkProductions.Inspector;
using SettingInspector.addons.settings_inspector.src.Inspectors;
using SettingInspector.addons.settings_inspector.Testing;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorHandler : Control
{
	[Export] private PackedScene _memberInspectorWindowScene;
	[Export] private bool _showTestingClass;

	[Export] public PackedScene? MemberWrapperScene;

	public static MemberInspectorHandler? Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance != null)
			QueueFree();
		else
			Instance = this;
	}

	public override void _Ready()
	{
		if (_showTestingClass)
			InspectorTesting();
	}

	private void InspectorTesting()
	{
		TestModel model = new();
		OpenClassInspector(model);
	}

	public MemberInspectorHandle<T> OpenClassInspector<T>() where T : new()
	{
		return OpenClassInspector<T>(new T());
	}

	public MemberInspectorHandle<T> OpenClassInspector<T>(T instance)
	{
		var inspectorWindow = _memberInspectorWindowScene.Instantiate<MemberInspectorWrapper>();
		AddChild(inspectorWindow);
		var wrapper = MemberWrapperScene.Instantiate<MemberWrapper>();
		inspectorWindow.AddChild(wrapper);
		var handle = new MemberInspectorHandle<T>(instance, wrapper);
		inspectorWindow.SetHandle(handle);
		return handle;
	}
}

public class MemberInspectorHandle<T> : IInspectorHandle
{
	public MemberInspectorHandle(T instance, MemberWrapper memberWrapper)
	{
		RootInspectorWrapper = memberWrapper;
		RootInspectorWrapper.SetMemberType(typeof(T));
		RootInspectorWrapper.MemberInspector.SetInstance(instance,
			new MemberUiInfo { AllowTabs = true, Scrollable = true },
			LayoutFlags.NotFoldable | LayoutFlags.NoBackground);
	}

	public event Action? OnClose;
	public MemberWrapper RootInspectorWrapper { get; }

	public void Apply()
	{
		if (RootInspectorWrapper.MemberInspector.TryRetrieveMember(out var val))
			OnApply?.Invoke((T)val!);
	}

	public void Close()
	{
		OnClose?.Invoke();
	}

	public event Action<T>? OnApply;
}

public interface IInspectorHandle
{
	public MemberWrapper RootInspectorWrapper { get; }
	public void Apply();
	public void Close();

	public event Action? OnClose;
}
