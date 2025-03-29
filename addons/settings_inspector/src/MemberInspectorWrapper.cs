using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorWrapper : Node
{
    [Export] private Button _applyButton;
    [Export] private Button _closeButton;

    private IInspectorHandle? _currentInspector;
    [Export] private Node _inspectorContainer;

    public void SetHandle(IInspectorHandle handle)
    {
        ResetInspector();
        handle.RootInspectorWrapper.GetParent()?.RemoveChild(handle.RootInspectorWrapper);
        _inspectorContainer.AddChild(handle.RootInspectorWrapper);
        _currentInspector = handle;
        _closeButton.Pressed += _currentInspector.Close;
        _applyButton.Pressed += _currentInspector.Apply;
        _currentInspector.OnClose += ResetInspector;
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

    public void SetVisible(bool visible)
    {
        if (GetNode("") is Window window) window.SetVisible(visible);
        if (GetNode("") is Control control) control.SetVisible(visible);
    }
}