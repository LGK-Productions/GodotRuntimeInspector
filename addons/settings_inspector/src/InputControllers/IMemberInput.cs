using System;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src;

public interface IMemberInput
{
	public void SetValue(object value);
	
	public object GetValue();

	public void SetEditable(bool editable);

	public event Action<object>? OnValueChanged;

    public virtual void SetElement(InspectorElement element)
    {
        OnSetElement(element);
        SetValue(element.Value);
        SetEditable(!element.MemberInfo.IsReadOnly);
    }

    void OnSetElement(InspectorElement element)
    {
        
    }
}
