using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorWindow : Window, IMemberInspectorWrapper
{
	[Export] private Button _applyButton;
	[Export] private Button _closeButton;

	[Export] private Node _inspectorContainer;

	public Node InspectorContainer => _inspectorContainer;
	public IInspectorHandle? CurrentInspector { get; set; }
	public Button ApplyButton => _applyButton;
	public Button CloseButton => _closeButton;
	public Node RootNode => this;

	void IMemberInspectorWrapper.SetHandleInternal(IInspectorHandle handle)
	{
		CloseRequested += CurrentInspector.Close;
	}

	void IMemberInspectorWrapper.ResetInspectorInternal()
	{
		CloseRequested -= CurrentInspector.Close;
	}

	public void SetVisible(bool visible)
	{
		Visible = visible;
	}

	public void SetContentScale(float scale)
	{
		ContentScaleFactor = scale;
	}
}

public interface IMemberInspectorWrapper
{
	protected Node InspectorContainer { get; }
	protected IInspectorHandle? CurrentInspector { get; set; }
	protected Button ApplyButton { get; }
	protected Button CloseButton { get; }
	public Node RootNode { get; }

	public void SetHandle(IInspectorHandle handle)
	{
		ResetInspector();
		handle.RootInspectorWrapper.GetParent()?.RemoveChild(handle.RootInspectorWrapper);
		InspectorContainer.AddChild(handle.RootInspectorWrapper);
		CurrentInspector = handle;
		CloseButton.Pressed += CurrentInspector.Close;
		ApplyButton.Pressed += CurrentInspector.Apply;
		CurrentInspector.OnClose += ResetInspector;
		SetHandleInternal(handle);
	}

	protected void SetHandleInternal(IInspectorHandle handle);

	private void ResetInspector()
	{
		if (CurrentInspector == null) return;
		ResetInspectorInternal();
		CloseButton.Pressed -= CurrentInspector.Close;
		ApplyButton.Pressed -= CurrentInspector.Apply;
		CurrentInspector.OnClose -= ResetInspector;
		CurrentInspector.RootInspectorWrapper.MemberInspector.Remove();
		CurrentInspector = null;
	}

	protected void ResetInspectorInternal();

	public void SetVisible(bool visible);

	public virtual void SetContentScale(float scale)
	{
	}
}
