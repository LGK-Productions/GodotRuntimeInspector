using System;
using System.Collections.Generic;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src;

public partial class ClassInspector : MemberInspector
{
	[Export] private PackedScene _memberGroupScene;
	[Export] private Node _memberParent;
    
    private object? _instance;
    
    List<(InspectorElement, MemberInspector)> _inspectors = new();
	
	public static PollingTickProvider TickProvider = new(1);

    protected override void SetValue(object? classInstance)
	{
        if (classInstance == null)
        {
            GD.Print("classInstance is null. Trying to create new instance.");
            try
            {
                classInstance = Activator.CreateInstance(InspectorElement.MemberInfo.Type);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                return;
            }
        }
        ClearInspector();
        _instance = classInstance;
		var inspector = Inspector.Attach(classInstance, TickProvider);

		Dictionary<string, MemberGroup> memberGroups = new();
		foreach (var element in inspector.Elements)
		{
			var memberInspector = MemberInspectorHandler.Instance!.GetInputScene(element.MemberInfo.Type).Instantiate<MemberInspector>();
			memberInspector.SetMember(element);
            memberInspector.ValueChanged += ChildValueChanged;
            _inspectors.Add((element, memberInspector));
			
			//Grouping Logic
			if (element.MemberInfo.GroupName == null)
				_memberParent.AddChild(memberInspector);
			else
			{
				if (memberGroups.TryGetValue(element.MemberInfo.GroupName, out var group))
					group.AddMember(memberInspector);
				else
				{
					var memberGroupNode = _memberGroupScene.Instantiate();
					_memberParent.AddChild(memberGroupNode);
					var memberGroup = (MemberGroup)memberGroupNode;
					memberGroup.SetGroup(element.MemberInfo.GroupName);
					memberGroups.Add(element.MemberInfo.GroupName, memberGroup);
					memberGroup.AddMember(memberInspector);
				}
			}
		}
	}
    
	public sealed class PollingTickProvider : ITickProvider
	{
		public event Action? Tick;

		public PollingTickProvider(float pollingRate)
		{
			var timer = new System.Timers.Timer(1/pollingRate);
			timer.Elapsed += (_, __) => Callable.From(() => Tick?.Invoke()).CallDeferred();
			timer.Start();
		}
	}

    private void ChildValueChanged()
    {
        OnValueChanged();
    }

    protected override object? GetValue()
    {
        //Write values back
        foreach (var (element, inspector) in _inspectors)
        {
            if (inspector.TryRetrieveMember(out object? value))
                element.Value = value;
        }

        return _instance;
    }

    public override void SetEditable(bool editable)
    {
        foreach (var (_, inspector) in _inspectors)
        {
            inspector.SetEditable(editable);
        }
    }

    private void ClearInspector()
    {
        foreach (var (element, inspector) in _inspectors)
        {
            inspector.ValueChanged -= ChildValueChanged;
            inspector.QueueFree();
        }
        _inspectors.Clear();
    }
}
