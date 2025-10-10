using System;
using Godot;
using LgkProductions.Inspector;
using Microsoft.Extensions.Logging;
using SettingInspector.addons.settings_inspector.Inspectors;
using SettingInspector.addons.settings_inspector.Testing;

namespace SettingInspector.addons.settings_inspector.Handlers;

public partial class MemberInspectorHandler : Control
{
    public const string Scope = "SettingInspector";

    private ILogger? _logger;
    
    [Export] private PackedScene? _memberInspectorWindowScene;
    [Export] private bool _showTestingClass;

    [Export] public PackedScene? MemberWrapperScene;

    public static MemberInspectorHandler? Instance { get; private set; }

    public static ILogger? Logger => Instance?._logger;

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

        var memberInspectorWrapper = _memberInspectorWindowScene!.Instantiate<IMemberInspectorWrapper>();
        AddChild(memberInspectorWrapper.RootNode);
        var wrapper = MemberWrapperScene!.Instantiate<MemberWrapper>();
        memberInspectorWrapper.RootNode.AddChild(wrapper);
        var handle = new MemberInspectorHandle<T>(instance, wrapper, memberInspectorWrapper);
        memberInspectorWrapper.SetHandle(handle);

        return handle;
    }

    /// <summary>
    ///     Constructs a node containing an inspector of the given instance
    /// </summary>
    /// <param name="instance">The instance to create an inspector for</param>
    /// <param name="flags">Additional layout flags</param>
    /// <param name="removeInset">Whether the inset should be removed</param>
    /// <typeparam name="T">The type of the instance</typeparam>
    /// <returns>A MemberWrapper containing the inspector</returns>
    /// <exception cref="NullReferenceException">Thrown if the instance is null or no MemberInspector instance exists</exception>
    public MemberWrapper ConstructMemberWrapper<T>(T instance,
        LayoutFlags flags = LayoutFlags.NotFoldable | LayoutFlags.NoLabel, bool removeInset = false)
    {
        if (instance is null) throw new NullReferenceException("instance is null");
        if (Instance is null) throw new NullReferenceException("No Member Inspector instance found");
        var memberWrapper = Instance.MemberWrapperScene!.Instantiate<MemberWrapper>();
        memberWrapper.SetMemberType(typeof(T));
        memberWrapper.MemberInspector.SetInstance(instance, MemberUiInfo.Default, flags);
        if (removeInset)
            memberWrapper.SetMargin(left: 0, right: 0);
        return memberWrapper;
    }
}

/// <summary>
/// A handle class exposing functionality to interact with the member inspector
/// </summary>
/// <typeparam name="T">type of the inspected instance</typeparam>
public class MemberInspectorHandle<T> : IInspectorHandle
{
    private bool _valid = true;

    public MemberInspectorHandle(T instance, MemberWrapper memberWrapper, IMemberInspectorWrapper root)
    {
        Root = root;
        RootInspectorWrapper = memberWrapper;
        RootInspectorWrapper.SetMemberType(typeof(T));
        if (instance is null) return;
        RootInspectorWrapper.MemberInspector.SetInstance(instance,
            new MemberUiInfo { AllowTabs = true, Scrollable = true },
            LayoutFlags.NotFoldable | LayoutFlags.NoBackground);
    }

    /// <summary>
    /// If set to true, the root will be set to invisible on close instead of being removed.
    /// This enables reopening the window without constructing it again.
    /// </summary>
    public bool Buffered { get; set; } = false;
    
    /// <summary>
    /// Root of this inspector. 
    /// </summary>
    public IMemberInspectorWrapper Root { get; }
    
    /// <summary>
    /// Gets called, if Close is called on this handle
    /// </summary>
    public event Action? OnClose;
    
    /// <summary>
    /// The root inspector wrapper
    /// </summary>
    public MemberWrapper RootInspectorWrapper { get; }

    /// <summary>
    /// Applies values of the ui to the inspected instance
    /// </summary>
    public void Apply()
    {
        if (!_valid) return;
        if (RootInspectorWrapper.MemberInspector.TryRetrieveMember(out var val))
            OnApply?.Invoke((T)val!);
    }

    /// <summary>
    /// Closes the inspector
    /// </summary>
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

    /// <summary>
    /// Reopens the inspector, only if buffered is enabled
    /// </summary>
    public void Reopen()
    {
        if (!_valid || !Buffered) return;
        Root.SetVisible(true);
    }

    /// <summary>
    /// Gets called, if apply is called on this handle
    /// </summary>
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