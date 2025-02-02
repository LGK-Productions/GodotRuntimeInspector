using System;
using System.Threading.Tasks;
using Godot;
using SettingInspector.addons.settings_inspector.Testing;

namespace SettingInspector.addons.settings_inspector.src;

public partial class ClassInspectorHandler : Control
{
	[Export] private Control _classInspectorContainer;
	[Export] private PackedScene _classInspectorScene;
	[Export] private PackedScene _classWindowScene;

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
	
	public async Task<T> OpenClassInspector<T>(T instance, bool asWindow = false)
	{
		var inspector = _classInspectorScene.Instantiate<ClassInspector>();
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
			return await inspector.EditClass(instance);
		}
		finally
		{
			if (!asWindow)
				_classInspectorContainer.RemoveChild(inspector);
			else 
				inspectorWindow!.QueueFree();
		}
	}
}
