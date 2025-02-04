using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using LgkProductions.Inspector;
using SettingInspector.addons.settings_inspector.src.InspectorCollections;

namespace SettingInspector.addons.settings_inspector.src;

public partial class MemberInspectorTabCollection : Control, IMemberInspectorCollection
{
	[Export] private TabContainer _tabContainer;
	[Export] private PackedScene _memberCollectionScene;

	private readonly Dictionary<string, IMemberInspectorCollection> _tabs = new();


	public IEnumerator<(InspectorElement, MemberInspector)> GetEnumerator()
	{
		foreach (var (_, collection) in _tabs)
		{
			foreach (var e in collection)
			{
				yield return (e);
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void SetMemberInspector(Inspector inspector)
	{
		//sort into tabs
		foreach (var element in inspector.Elements)
		{
			AddElement(element);
		}
	}

	public void AddElement(InspectorElement element)
	{
		var tab = element.MemberInfo.TabName ?? "Default";
		if (_tabs.TryGetValue(tab, out var collection))
		{
			collection.AddElement(element);
		}
		else
		{
			var collectionNode = _memberCollectionScene.Instantiate();
			var memberCollection = (IMemberInspectorCollection)collectionNode;
			collectionNode.Name = tab;
			_tabContainer.AddChild(collectionNode);
			memberCollection.AddElement(element);
			_tabs.Add(tab, memberCollection);
			memberCollection.ValueChanged += OnChildValueChanged;
		}

		_tabContainer.TabsVisible = _tabs.Count > 1;
	}

	public void WriteBack()
	{
		foreach (var (_, collection) in _tabs)
		{
			collection.WriteBack();
		}
	}

	public void Clear()
	{
		foreach (var (_, collection) in _tabs)
		{
			collection.Clear();
		}
		_tabs.Clear();
	}

	public void SetEditable(bool editable)
	{
		foreach (var (_, collection) in _tabs)
		{
			collection.SetEditable(editable);
		}
	}

	public void SetScrollable(bool scrollable)
	{
		foreach (var (_, collection) in _tabs)
		{
			collection.SetScrollable(scrollable);
		}
	}

	private void OnChildValueChanged()
	{
		ValueChanged?.Invoke();
	}
	
	

	public event Action? ValueChanged;
}
