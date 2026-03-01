using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LgkProductions.Inspector;
using LgkProductions.Inspector.MetaData;
using Microsoft.Extensions.Logging;
using RuntimeInspector.Handlers;
using RuntimeInspector.Util;

namespace RuntimeInspector.Inspectors.Collections;

public partial class DictionaryInspector : MemberInspector
{
	[Export] private FoldableContainer? _foldableContainer;
	[Export] private PackedScene? _dictionaryElementScene;
	[Export] private Control? _memberParent;
	[Export] private Button? _addButton;
	[Export] private Control? _keyParent;

	private readonly List<DictionaryElement> _dictionaryElements = new ();

	private IDictionary _dictionary;
	
	private MemberInspector? _currentKeyInspector;
	
	private Type? _keyType;
	private Type? _valueType;
	private List<Type>? _assignableKeyTypes;
	private List<Type>? _assignableValueTypes;
	
	private static LayoutFlags _layoutFlags = LayoutFlags.NoLabel;
	
	protected override void OnInitialize()
	{
		_addButton!.Pressed += AddNewDictionaryEntry;
	}

	protected override void OnRemove()
	{
		_addButton!.Pressed -= AddNewDictionaryEntry;
	}

	private void AddNewDictionaryEntry()
	{
		var key = _currentKeyInspector.TryRetrieveMember(out var result) ? result : null;
		var t = _valueType;
		if (_valueType.IsAbstract)
		{
			if (_assignableValueTypes.Count <= 0) return;
			t = _assignableValueTypes[0];
		}
		if (!Util.Util.TryCreateInstance(t, out var value))
		{
			MemberInspectorHandler.Logger.Log(LogLevel.Error, "Could not create value of type {valueType}", _valueType);
			return;
		}
		if (_dictionary.Contains(key))
		{
			MemberInspectorHandler.Logger.Log(LogLevel.Error, "Tried to add already existing key to dictionary!");
			return;
		}
		AppendDictionaryElement(key, value);
		_dictionary.Add(key, value);
		_currentKeyInspector.Remove();
		CreateCurrentKeyInspector();
		OnValueChanged(new ValueChangeTree(this, _dictionary));
	}

	protected override void OnSetMetaData(MetaDataMember member)
	{
		base.OnSetMetaData(member);
		_foldableContainer.Title = member.Name;
	}

	protected override void SetLayoutFlags(LayoutFlags flags)
	{
		base.SetLayoutFlags(flags);
		_foldableContainer.Folded = !flags.HasFlag(LayoutFlags.ExpandedInitially);
		if (flags.HasFlag(LayoutFlags.NoLabel))
			_foldableContainer.Title = "";
		/*if (flags.HasFlag(LayoutFlags.NoBackground))
			_foldableContainer.AddThemeStyleboxOverride("panel", new StyleBoxFlat() {BgColor = new Color(0, 0, 0, 0)});*/
		if (flags.IsSet(LayoutFlags.NoBackground))
		{
			_foldableContainer?.SetVisible(false);
			_memberParent?.GetParent()?.SetOwner(null);
			_memberParent?.GetParent()?.Reparent(this);
		}
	}

	protected override void SetValueInternal(object value)
	{
		if (value is not IDictionary dictionary)
		{
			MemberInspectorHandler.Logger.Log(LogLevel.Error, "{value} is not a dictionary", value);
			return;
		}
		_dictionary = dictionary;
		
		//TODO: this does not handle all cases
		_keyType = value.GetType().GenericTypeArguments[0];
		_valueType = value.GetType().GenericTypeArguments[1];
		
		if (_keyType.IsAbstract)
			_assignableKeyTypes = Util.Util.GetAssignableTypes(_keyType).ToList();
		if (_keyType.IsAbstract)
			_assignableValueTypes = Util.Util.GetAssignableTypes(_valueType).ToList();
		
		foreach (var key in dictionary.Keys)
		{
			AppendDictionaryElement(key, dictionary[key]);
		}

		CreateCurrentKeyInspector();
		
	}

	private void CreateCurrentKeyInspector()
	{
		var t = _keyType;
		if (_keyType.IsAbstract)
		{
			if (_assignableKeyTypes.Count <= 0) return;
			t = _assignableKeyTypes[0];
		}
		if (_keyType == null || !Util.Util.TryCreateInstance(t, out var currentKey)) return;
		var keyWrapper = MemberInspectorHandler.Instance?.MemberWrapperScene?.Instantiate<MemberWrapper>();
		keyWrapper.SetMemberType(t);
		var info = MemberUiInfo.Default;
		if (_keyType.IsAbstract)
			info = info with{ ParentType = _keyType};
		keyWrapper.MemberInspector.SetInstance(currentKey, info, _layoutFlags);
		_currentKeyInspector = keyWrapper.MemberInspector;
		_keyParent.AddChild(keyWrapper);
	}

	private void AppendDictionaryElement(object key, object? value)
	{
		if (_keyType == null || _valueType == null) return;
		
		var keyWrapper = MemberInspectorHandler.Instance?.MemberWrapperScene?.Instantiate<MemberWrapper>();
		keyWrapper.SetMemberType(_keyType);
		var valueWrapper = MemberInspectorHandler.Instance?.MemberWrapperScene?.Instantiate<MemberWrapper>();
		valueWrapper.SetMemberType(_valueType);
		
		var dictionaryElementInstance = _dictionaryElementScene!.Instantiate<DictionaryElement>();
		var keyMemberUiInfo = MemberUiInfo.Default;
		var valueMemberUiInfo = MemberUiInfo.Default;
		if (key.GetType() != _keyType)
			keyMemberUiInfo = keyMemberUiInfo with { ParentType = _keyType };
		if (value.GetType() != _valueType)
			valueMemberUiInfo = valueMemberUiInfo with { ParentType = _valueType };

		keyWrapper.MemberInspector.SetInstance(key, keyMemberUiInfo, _layoutFlags);
		valueWrapper.MemberInspector.SetInstance(value, valueMemberUiInfo, _layoutFlags);
		
		dictionaryElementInstance.SetMemberInspector(keyWrapper, valueWrapper, this);
		_memberParent!.AddChild(dictionaryElementInstance);
		_dictionaryElements.Add(dictionaryElementInstance);
		dictionaryElementInstance.ValueChanged += OnChildValueChanged;
	}

	protected override void Clear()
	{
		base.Clear();
		while (_dictionaryElements.Count > 0)
		{
			RemoveDictionaryElement(_dictionaryElements[0]);
		}
		_dictionary = null;
		_currentKeyInspector?.Remove();
		_currentKeyInspector = null;
		_keyType = null;
		_valueType = null;
	}

	public void RemoveDictionaryElement(DictionaryElement element, bool removeFromDict = false)
	{
		_dictionaryElements.Remove(element);
		if (removeFromDict)
		{
			var key = element.GetKey();
			if (key != null && _dictionary.Contains(key))
				_dictionary.Remove(key);
		}
		element.ValueChanged -= OnChildValueChanged;
		element.Remove();
	}

	private void OnChildValueChanged(ValueChangeTree tree)
	{
		OnValueChanged(new ValueChangeTree(this, _dictionary, tree));
	}
	
	protected override object? GetValue()
	{
		foreach (var element in _dictionaryElements)
		{
			element.GetValue();
		}
		return _dictionary;
	}
}
