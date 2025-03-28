using System;
using System.Linq;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.Inspectors;

namespace SettingInspector.addons.settings_inspector.src;

public abstract partial class MemberInspector : Control
{
	[Export] private Label _label;
	[Export] private MarginContainer _marginContainer;
	[Export] private Control _background;

	public Type? ValueType { get; protected set; }

	protected bool Editable = true;
	protected MemberUiInfo MemberUiInfo;
	protected LayoutFlags LayoutFlags;

	private InspectorElement? _element;
	private MemberWrapper? _wrapper;

	private bool _initialized = false;

	public void SetMember(InspectorElement iElement)
	{
		_element = iElement;
		OnSetMetaData(_element.MemberInfo);
		var memberUiInfo = MemberUiInfo.Default;
		ValueType = iElement.MemberInfo.Type;
		var isAssignableType = ValueType.IsAbstract || ValueType.IsInterface;
		if (isAssignableType)
		{
			memberUiInfo = memberUiInfo with { parentType = iElement.MemberInfo.Type };
			var availableTypes = Util.GetAssignableTypes(ValueType).ToArray();
			if (availableTypes.Length > 0)
			{
				ValueType = availableTypes[0];
			}
		}


		var value = _element.Value;
		if (value == null && !Util.TryCreateInstance(ValueType, out value))
		{
			GD.Print("Value is null, could not create instance.");
			return;
		}

		SetInstance(value!, memberUiInfo, iElement.MemberInfo.LayoutFlags);

		_element.ValueChanged += UpdateMemberInputValue;
	}

	public void SetInstance(object value) => SetInstance(value, MemberUiInfo.Default);

	public void SetInstance(object value, MemberUiInfo memberUiInfo, LayoutFlags flags = LayoutFlags.Default)
	{
		if (!_initialized)
			OnInitialize();

		ValueType = value.GetType();
		if (_element == null)
			_label.Text = ValueType.Name;

		SetMemberUiInfo(memberUiInfo);
		SetLayoutFlags(flags);
		SetValue(value);
	}


	protected virtual void SetMemberUiInfo(MemberUiInfo memberUiInfo)
	{
		MemberUiInfo = memberUiInfo;
	}

	protected virtual void SetLayoutFlags(LayoutFlags flags)
	{
		LayoutFlags = flags;
		_label?.SetVisible(!flags.IsSet(LayoutFlags.NoLabel));
		_background?.SetVisible(!flags.IsSet(LayoutFlags.NoBackground));
	}

	protected abstract object? GetValue();

	protected virtual void SetValue(object value)
	{
		Clear();
	}

	public virtual void SetEditable(bool editable)
	{
		Editable = editable;
	}

	protected virtual void OnSetMetaData(MetaDataMember member)
	{
		_label.Text = _element.MemberInfo.DisplayName;
		_label.TooltipText = _element.MemberInfo.Description;
		if (member.TryGetMetaData(new MetaDataKey<float>("LabelSize"), out var labelSizeMultiplier))
			_label.SizeFlagsStretchRatio = labelSizeMultiplier;
		_wrapper?.SetMargin((int)member.Spacing.Top, (int)member.Spacing.Botton, (int)member.Spacing.Left,
			(int)member.Spacing.Right);

		SetEditable(!_element.MemberInfo.IsReadOnly);
	}

	public void SetWrapper(MemberWrapper wrapper)
	{
		_wrapper = wrapper;
	}

	protected virtual void Clear()
	{
	}

	protected virtual void OnInitialize()
	{
	}

	protected virtual void OnRemove()
	{
	}

	public void Remove()
	{
		Clear();
		if (_element != null)
		{
			_element.ValueChanged -= UpdateMemberInputValue;
			_element = null;
		}

		OnRemove();
		QueueFree();
	}

	public event Action ValueChanged;

	protected void OnValueChanged()
	{
		ValueChanged?.Invoke();
	}

	private void UpdateMemberInputValue(object instance, MetaDataMember member, object? value)
	{
		if (value == null) return;
		SetValue(value);
	}

	public bool TryRetrieveMember(out object? result)
	{
		result = null;
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
