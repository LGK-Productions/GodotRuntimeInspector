using System;
using System.Threading.Tasks;
using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public partial class ClassInspector : Node
{
    [Export] private Control _classContainer;
    [Export] private Window _classWindow;

    private bool _isOpen;
    
    public Task<T> EditClass<T>() where T : new()
    {
        return EditClass<T>(new T());
    }

    public async Task<T> EditClass<T>(T classInstance) where T : new()
    {
        if (_isOpen)
        {
            GD.Print("Tried to open class editor, but it is already open.");
            throw new OperationCanceledException();
        }
        throw new OperationCanceledException();
    }
}