using System;
using Godot;
using LgkProductions.Inspector;
using Microsoft.Extensions.Logging;
using SettingInspector.addons.settings_inspector.src.Inspectors;
using SettingInspector.addons.settings_inspector.Testing;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorHandler : Control
{
    [Export] private PackedScene _memberInspectorWindowScene;
    [Export] private bool _showTestingClass;

    [Export] public PackedScene? MemberWrapperScene;

    public static MemberInspectorHandler? Instance { get; private set; }
    
    public static ILogger? Logger => Instance?._logger;

    private ILogger? _logger;

    public const string Scope = "SettingInspector";

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
    ///     Opens a new inspector of the given <paramref name="instance" />
    /// </summary>
    /// <param name="instance">The instance to open an inspector for</param>
    /// <param name="buffered">
    ///     Whether the instance should be buffered instead of being destroyed upon closing.
    ///     Opening an Inspector with an already buffered instance reopens the buffered window instead of creating a new one.
    /// </param>
    /// <typeparam name="T">The type of the opened inspector.</typeparam>
    /// <returns>An <see cref="MemberInspectorHandle{T}" /> holding references to the inspector.</returns>
    /// <exception cref="NullReferenceException">Thrown if the instance is null</exception>
    public MemberInspectorHandle<T> OpenClassInspectorWindow<T>(T instance)
    {
        if (instance == null) throw new NullReferenceException("instance is null");

        var memberInspectorWrapper = _memberInspectorWindowScene.Instantiate<IMemberInspectorWrapper>();
        AddChild(memberInspectorWrapper.RootNode);
        var wrapper = MemberWrapperScene.Instantiate<MemberWrapper>();
        memberInspectorWrapper.RootNode.AddChild(wrapper);
        var handle = new MemberInspectorHandle<T>(instance, wrapper, memberInspectorWrapper);
        memberInspectorWrapper.SetHandle(handle);

        return handle;
    }

    public MemberWrapper ConstructMemberWrapper<T>(T instance, LayoutFlags flags = LayoutFlags.NotFoldable | LayoutFlags.NoLabel, bool removeInset = false)
    {
        var memberWrapper = MemberInspectorHandler.Instance.MemberWrapperScene.Instantiate<MemberWrapper>();
        memberWrapper.SetMemberType(typeof(T));
        memberWrapper.MemberInspector.SetInstance(instance, MemberUiInfo.Default, flags);
        if (removeInset)
            memberWrapper.SetMargin(left: 0, right: 0);
        return memberWrapper;
    }
}

public class MemberInspectorHandle<T> : IInspectorHandle
{
    private bool _valid = true;

    public MemberInspectorHandle(T instance, MemberWrapper memberWrapper, IMemberInspectorWrapper root)
    {
        Root = root;
        RootInspectorWrapper = memberWrapper;
        RootInspectorWrapper.SetMemberType(typeof(T));
        RootInspectorWrapper.MemberInspector.SetInstance(instance,
            new MemberUiInfo { AllowTabs = true, Scrollable = true },
            LayoutFlags.NotFoldable | LayoutFlags.NoBackground);
    }

    public bool Buffered { get; set; } = false;
    public IMemberInspectorWrapper Root { get; }
    public event Action? OnClose;
    public MemberWrapper RootInspectorWrapper { get; }

    public void Apply()
    {
        if (!_valid) return;
        if (RootInspectorWrapper.MemberInspector.TryRetrieveMember(out var val))
            OnApply?.Invoke((T)val!);
    }

    public void Close()
    {
        if (!_valid) return;
        if (Buffered)
        {
            Root.SetVisible(false);
        }
        else
        {
            Root.RootNode.QueueFree();
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
    public IMemberInspectorWrapper Root { get; }
    public void Apply();
    public void Close();
    public void Reopen();

    public event Action? OnClose;
}
