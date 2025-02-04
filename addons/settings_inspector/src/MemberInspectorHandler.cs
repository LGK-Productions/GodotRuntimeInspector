using System;
using System.Threading.Tasks;
using Godot;
using SettingInspector.addons.settings_inspector.Testing;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorHandler : Control
{
	[Export] private Control _classInspectorContainer;
	[Export] private PackedScene _memberInspectorWrapperScene;
	[Export] private PackedScene _classWindowScene;
	
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
		InspectorTesting();
	}

	private async void InspectorTesting()
	{
		TestModel model = new();
		Task.Delay(5000).ContinueWith(x =>
		{
			return model.TestOrder0++;
		});
		while (true)
		{
			try
			{
				model = await OpenClassInspector(model, true);
				break;
			}
			catch (OperationCanceledException e)
			{
				GD.Print("editor cancelled");
			}
		}
	}

	public Task<T> OpenClassInspector<T>() where T : new()
	{
		return OpenClassInspector<T>(new T());
	}
	
	public async Task<T> OpenClassInspector<T>(T instance, bool asWindow = false, bool readOnly = false)
	{
		var inspector = _memberInspectorWrapperScene.Instantiate<MemberInspectorButtonWrapper>();
		Window inspectorWindow = null;
		if (!asWindow)
			_classInspectorContainer.AddChild(inspector);
		else
		{
			inspectorWindow = _classWindowScene.Instantiate<Window>();
			AddChild(inspectorWindow);
			inspectorWindow.AddChild(inspector);
		}
		
		try
		{
			return await inspector.SetInspector(instance, readOnly);
		}
		finally
		{
			if (!asWindow)
				_classInspectorContainer.RemoveChild(inspector);
			else 
				inspectorWindow!.QueueFree();
		}
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
		if (!inputType.IsPrimitive && inputType != typeof(string))
		{
			if (InspectorScenes.Count > 5)
				return InspectorScenes[5];
		}

		return DefaultInspectorScene;
	}
}
