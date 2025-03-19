using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorWrapper : Node
{
	[Export] private Button _closeButton;
	[Export] private Button _applyButton;
	[Export] private Node _inspectorContainer;
	
	private IInspectorHandle? _currentInspector;

	public void SetHandle(IInspectorHandle handle, bool destroyOnClosed = true)
	{
		ResetInspector();
		handle.RootInspectorWrapper.GetParent()?.RemoveChild(handle.RootInspectorWrapper);
		_inspectorContainer.AddChild(handle.RootInspectorWrapper);
		_currentInspector = handle;
		_closeButton.Pressed += _currentInspector.Close;
		_applyButton.Pressed += _currentInspector.Apply;
		_currentInspector.OnClose += destroyOnClosed ? CloseInspector : ResetInspector;
	}

	private void ResetInspector()
	{
		if (_currentInspector == null) return;
		_closeButton.Pressed -= _currentInspector.Close;
		_applyButton.Pressed -= _currentInspector.Apply;
		_currentInspector.OnClose -= ResetInspector;
		_currentInspector.RootInspectorWrapper.MemberInspector.Remove();
		_currentInspector = null;
	}

	private void CloseInspector()
	{
		if (_currentInspector != null)
		{
			_closeButton.Pressed -= _currentInspector.Close;
			_applyButton.Pressed -= _currentInspector.Apply;
			_currentInspector.OnClose -= CloseInspector;
			_currentInspector.RootInspectorWrapper.MemberInspector.Remove();
			_currentInspector = null;
		}
		QueueFree();
	}
}
