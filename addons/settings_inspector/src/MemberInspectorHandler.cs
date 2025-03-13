using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using LgkProductions.Inspector;
using SettingInspector.addons.settings_inspector.Testing;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorHandler : Control
{
	[Export] private PackedScene _memberInspectorWindowScene;
	[Export] private bool _showTestingClass;
	
	[Export] public PackedScene DefaultInspectorScene;
	[Export] public Godot.Collections.Array<PackedScene> InspectorScenes;

	
	public static MemberInspectorHandler? Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance != null)
			QueueFree();
		else
		{
			Instance = this;
		}
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
		var handle = new MemberInspectorHandle<T>(instance);
		inspectorWindow.SetHandle(handle);
		return handle;
	}
	
	public PackedScene GetInputScene(Type inputType)
	{
		if (inputType == typeof(bool))
		{
			if (InspectorScenes.Count > 0)
				return Instance.InspectorScenes[0];
		}
		if (inputType == typeof(int))
		{
			if (InspectorScenes.Count > 1)
				return InspectorScenes[1];
		}
		if (inputType == typeof(float))
		{
			if (InspectorScenes.Count > 2)
				return InspectorScenes[2];
		}
		if (inputType == typeof(double))
		{
			if (InspectorScenes.Count > 3)
				return InspectorScenes[3];
		}
		if (inputType.IsEnum)
		{
			if (InspectorScenes.Count > 4)
				return InspectorScenes[4];
		}

		if (inputType.IsGenericType && inputType.GetGenericTypeDefinition() == typeof(List<>))
		{
			if (InspectorScenes.Count > 6)
				return InspectorScenes[6];
		}
		if (!inputType.IsPrimitive && inputType != typeof(string))
		{
			if (InspectorScenes.Count > 5)
				return InspectorScenes[5];
		}

		return DefaultInspectorScene;
	}
}

public class MemberInspectorHandle<T> : IInspectorHandle
{
	public event Action<T>? OnApply;
	public event Action? OnClose;
	
	public MemberInspector RootInspector { get; }
	
	public MemberInspectorHandle(T instance)
	{
		RootInspector = MemberInspectorHandler.Instance.GetInputScene(typeof(T)).Instantiate<MemberInspector>();
		RootInspector.SetInstance(instance, new () { AllowTabs = true, Scrollable = true }, LayoutFlags.NotFoldable | LayoutFlags.NoBackground);
	}
	public void Apply()
	{
		if (RootInspector.TryRetrieveMember(out var val))
			OnApply?.Invoke((T)val!);
	}

	public void Close()
	{
		OnClose?.Invoke();
	}
}

public interface IInspectorHandle
{
	public void Apply();
	public void Close();
	
	public event Action? OnClose;
	
	public MemberInspector RootInspector { get; }
}
