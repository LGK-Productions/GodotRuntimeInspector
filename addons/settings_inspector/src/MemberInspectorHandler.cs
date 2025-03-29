using System;
using System.Collections.Generic;
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
        OpenClassInspectorWindow(model);
    }

    public MemberInspectorHandle<T> OpenClassInspectorWindow<T>() where T : new()
    {
        return OpenClassInspectorWindow<T>(new T());
    }

    /// <summary>
    /// Opens a new inspector of the given <paramref name="instance"/>
    /// </summary>
    /// <param name="instance">The instance to open an inspector for</param>
    /// <param name="buffered">Whether the instance should be buffered instead of being destroyed upon closing.
    /// Opening an Inspector with an already buffered instance reopens the buffered window instead of creating a new one.</param>
    /// <typeparam name="T">The type of the opened inspector.</typeparam>
    /// <returns>An <see cref="MemberInspectorHandle{T}"/> holding references to the inspector.</returns>
    /// <exception cref="NullReferenceException">Thrown if the instance is null</exception>
    public MemberInspectorHandle<T> OpenClassInspectorWindow<T>(T instance)
    {
        if (instance == null) throw new NullReferenceException("instance is null");
        
        var memberInspectorWrapper = _memberInspectorWindowScene.Instantiate<MemberInspectorWrapper>();
        AddChild(memberInspectorWrapper);
        var wrapper = MemberWrapperScene.Instantiate<MemberWrapper>();
        memberInspectorWrapper.AddChild(wrapper);
        var handle = new MemberInspectorHandle<T>(instance, wrapper, memberInspectorWrapper);
        memberInspectorWrapper.SetHandle(handle);

        return handle;
    }
}

public class MemberInspectorHandle<T> : IInspectorHandle
{
    public MemberInspectorHandle(T instance, MemberWrapper memberWrapper, MemberInspectorWrapper root)
    {
        Root = root;
        RootInspectorWrapper = memberWrapper;
        RootInspectorWrapper.SetMemberType(typeof(T));
        RootInspectorWrapper.MemberInspector.SetInstance(instance,
            new MemberUiInfo { AllowTabs = true, Scrollable = true },
            LayoutFlags.NotFoldable | LayoutFlags.NoBackground);
    }

    public bool Buffered { get; set; } = false;
    public MemberInspectorWrapper Root { get; }
    public event Action? OnClose;
    public MemberWrapper RootInspectorWrapper { get; }

    private bool _valid = true;

    public void Apply()
    {
        if (!_valid) return;
        if (RootInspectorWrapper.MemberInspector.TryRetrieveMember(out var val))
            OnApply?.Invoke((T)val!);
    }

    public void Close()
    {
        if(!_valid) return;
        if (Buffered)
            Root.SetVisible(false);
        else
        {
            Root.QueueFree();
            _valid = false;
        }
        OnClose?.Invoke();
    }

    public void Reopen()
    {
        if (!_valid || !Buffered) return;
        Root.SetVisible(true);
    }

    public event Action<T>? OnApply;
}

public interface IInspectorHandle
{
    public MemberWrapper RootInspectorWrapper { get; }
    public MemberInspectorWrapper Root { get; }
    public void Apply();
    public void Close();
    public void Reopen();

    public event Action? OnClose;
}