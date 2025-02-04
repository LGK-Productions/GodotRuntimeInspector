using System;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src;

public abstract partial class MemberInspector : Control
{
	[Export] private Label _label;
    
    protected Type? ValueType;
    protected bool Editable = true;
	
	public void SetMember(InspectorElement iElement)
	{
		_label.Text = iElement.MemberInfo.DisplayName;
		_label.TooltipText = iElement.MemberInfo.Description;
        
        ValueType = iElement.MemberInfo.Type;
        
        SetEditable(!iElement.MemberInfo.IsReadOnly);

		
		OnSetMetaData(iElement.MemberInfo);
		
		iElement.ValueChanged += UpdateMemberInputValue;
		SetValue(iElement.Value);
	}

	/// <summary>
	/// Sets an input value, removing the name label description, etc
	/// </summary>
	/// <param name="value"></param>
	public void SetInputValue(object? value)
	{
		_label.Visible = false;
        ValueType = value?.GetType();
		SetValue(value);
	}
	
	protected abstract object? GetValue();
	protected abstract void SetValue(object? value);

    public virtual void SetEditable(bool editable)
    {
        Editable = editable;
    }
	protected virtual void OnSetMetaData(MetaDataMember member){}
	public event Action ValueChanged;
	protected void OnValueChanged()
	{
		ValueChanged?.Invoke();
	}

	private void UpdateMemberInputValue(object instance, MetaDataMember member, object? value)
	{
		SetValue(value);
	}

	public bool TryRetrieveMember(out object? result)
	{
		result = default;
		if (ValueType == null)
		{
			GD.PrintErr("Could not retrieve member, due to no type being set");
			return false;
		}

		try
		{
			result = Convert.ChangeType(GetValue(), ValueType);
			return true;
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
		}

		return false;
	}
}
