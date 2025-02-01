using LgkProductions.Inspector.MetaData;
using System.ComponentModel;

namespace LgkProductions.Inspector;

public sealed class InspectorElement : IDisposable
{
    public object Instance { get; }
    public InspectorMember MemberInfo { get; }

    readonly ITickProvider? _tickProvider;
    private InspectorElement(object instance, InspectorMember member, ITickProvider? tickProvider)
    {
        Instance = instance;
        MemberInfo = member;

        _tickProvider = tickProvider;

        if (instance is INotifyPropertyChanged notify)
            notify.PropertyChanged += OnInstancePropertyChanged;

        if (tickProvider is not null)
            tickProvider.Tick += OnTick;
    }

    public object? Value
    {
        get => MemberInfo.GetValue(Instance);
        set => MemberInfo.SetValue(Instance, value);
    }

    public event ValueChangedEventHandler? ValueChanged;

    #region Notify
    public static InspectorElement AttachNotify<TInstance>(TInstance instance, InspectorMember member) where TInstance : INotifyPropertyChanged
    {
        if (!member.IsDeclaredIn(instance))
            throw new ArgumentException("The instance does not declare the member.", nameof(instance));

        return new(instance, member, tickProvider: null);
    }

    private void OnInstancePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != MemberInfo.Name)
            return;

        ValueChanged?.Invoke(Instance, MemberInfo, MemberInfo.GetValue(Instance));
    }
    #endregion Notify

    #region Polling
    public static InspectorElement AttachPoll(object instance, InspectorMember member, ITickProvider tickProvider)
    {
        if (!member.IsDeclaredIn(instance))
            throw new ArgumentException("The instance does not declare the member.", nameof(instance));

        return new(instance, member, tickProvider);
    }

    object? _cachedValue;
    void OnTick()
    {
        var newValue = MemberInfo.GetValue(Instance);
        if (Equals(newValue, _cachedValue))
            return;

        _cachedValue = newValue;
        ValueChanged?.Invoke(Instance, MemberInfo, newValue);
    }
    #endregion Polling

    public void Dispose()
    {
        if (Instance is INotifyPropertyChanged notify)
            notify.PropertyChanged -= OnInstancePropertyChanged;

        if (_tickProvider is not null)
            _tickProvider.Tick -= OnTick;
    }
}

public delegate void ValueChangedEventHandler(object instance, InspectorMember member, object? newValue);
