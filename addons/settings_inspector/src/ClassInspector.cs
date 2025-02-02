using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src;

public partial class ClassInspector : Node
{
	[Export] private PackedScene _memberScene;
	[Export] private PackedScene _memberGroupScene;
	[Export] private Node _memberParent;
	[Export] private Label _title;
	[Export] private Button _confirmButton;
	[Export] private Button _cancelButton;
    
    public static PollingTickProvider TickProvider = new(1);


	private TaskCompletionSource? _tcs;

	public override void _Ready()
	{
		_confirmButton.Pressed += () => _tcs?.SetResult();
		_cancelButton.Pressed += () => _tcs?.SetCanceled();
	}

	public async Task<T> EditClass<T>(T classInstance)
	{
		if (_tcs != null)
		{
			GD.Print("Tried to open class editor, but it is already open.");
			throw new OperationCanceledException();
		}

		_tcs = new TaskCompletionSource();
		var inspector = Inspector.Attach(classInstance, TickProvider);

		_title.Text = classInstance.GetType().Name;
		List<Action> retrievalActions = new();
		Dictionary<string, MemberGroup> memberGroups = new();
		foreach (var element in inspector.Elements)
		{
			var memberInspector = _memberScene.Instantiate<MemberInspector>();
			memberInspector.SetMember(element);
			
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
            
            retrievalActions.Add(() =>
            {
                if (memberInspector.TryRetrieveMember(out var value))
                    element.Value = value;
            });
		}

		try
		{
			await _tcs.Task;

			foreach (var action in retrievalActions)
			{
				action.Invoke();
			}

			return classInstance;
		}
		finally
		{
			_tcs = null;
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
}
