using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src;

public partial class ClassInspector : Node
{
    [Export] private Control _classContainer;
    [Export] private Window _classWindow;
    [Export] private PackedScene _memberScene;
    [Export] private Button _confirmButton;
    [Export] private Button _cancelButton;

    private TaskCompletionSource? _tcs;

    public override void _Ready()
    {
        _confirmButton.Pressed += () => _tcs?.SetResult();
        _cancelButton.Pressed += () => _tcs?.SetCanceled();
    }

    public Task<T> EditClass<T>() where T : new()
    {
        return EditClass<T>(new T());
    }

    public async Task<T> EditClass<T>(T classInstance) where T : new()
    {
        if (_tcs != null)
        {
            GD.Print("Tried to open class editor, but it is already open.");
            throw new OperationCanceledException();
        }

        _tcs = new TaskCompletionSource();
        TestTickProvider testTickProvider = new();
        var inspector = Inspector.Attach(classInstance, testTickProvider);

        List<Action> retrievalActions = new();
        foreach (var element in inspector.Elements)
        {
            var memberInspector = _memberScene.Instantiate<MemberInspector>();
            memberInspector.SetMember(element);
            retrievalActions.Add(() =>
            {
                if (memberInspector.TryRetrieveMember<object>(out var value))
                    element.Value = value;
            });
        }

        await _tcs.Task;

        foreach (var action in retrievalActions)
        {
            action.Invoke();
        }
        
        return classInstance;
    }


    internal sealed class TestTickProvider : ITickProvider
    {
        public event Action? Tick;

        public void TriggerTick()
            => Tick?.Invoke();
    }
}