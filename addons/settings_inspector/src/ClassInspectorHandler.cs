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
		OpenClassInspectorWindow<TestModel>();
	}

	public async Task<T> OpenClassInspectorWindow<T>() where T : new()
	{
		var inspector = _classInspectorScene.Instantiate<ClassInspector>();
		_classInspectorContainer.AddChild(inspector);
		return await inspector.EditClass<T>();
	}
}
