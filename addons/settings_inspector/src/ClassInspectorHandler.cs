using System;
using System.Threading.Tasks;
using Godot;
using SettingInspector.addons.settings_inspector.Testing;

namespace SettingInspector.addons.settings_inspector.src;

public partial class ClassInspectorHandler : Control
{
	[Export] private Control _classInspectorContainer;
	[Export] private Window _classWindow;
	[Export] private PackedScene _classInspectorScene;

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
				model = await OpenClassInspectorWindow(model);
				break;
			}
			catch (OperationCanceledException e)
			{
				GD.Print("editor cancelled");
			}
		}
	}

	public Task<T> OpenClassInspectorWindow<T>() where T : new()
	{
		return OpenClassInspectorWindow<T>(new T());
	}
	
	public async Task<T> OpenClassInspectorWindow<T>(T instance)
	{
		var inspector = _classInspectorScene.Instantiate<ClassInspector>();
		_classInspectorContainer.AddChild(inspector);
		try
		{
			return await inspector.EditClass(instance);
		}
		finally
		{
			_classInspectorContainer.RemoveChild(inspector);
		}
	}
}
