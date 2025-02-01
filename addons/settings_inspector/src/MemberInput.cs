using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public interface IMemberInput
{
    public void SetValue(object value);
    
    public bool TryGetValue<T>(out T? value);
}