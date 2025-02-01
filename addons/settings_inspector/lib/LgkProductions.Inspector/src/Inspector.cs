using LgkProductions.Inspector.MetaData;
using System.ComponentModel;

namespace LgkProductions.Inspector;

public sealed class Inspector : IDisposable
{
    public required IReadOnlyList<InspectorElement> Elements { get; init; }

    public void Dispose()
    {
        List<Exception>? exceptions = null;
        foreach (var element in Elements)
        {
            try
            {
                element.Dispose();
            }
            catch (Exception ex)
            {
                exceptions ??= [];
                exceptions.Add(ex);
            }
        }

        if (exceptions is not null)
            throw new AggregateException(exceptions);
    }

    public static Inspector Attach(object instance, ITickProvider tickProvider)
    {
        var type = instance.GetType();

        var metaData = MetaDataCollector.Collect(type, new()
        {
            IncludeFields = true
        });

        INotifyPropertyChanged? notify = instance as INotifyPropertyChanged;

        List<InspectorElement> elements = [];
        foreach (var member in metaData)
        {
            if (member.IsReadOnly)
                continue;

            if (notify is not null)
            {
                elements.Add(InspectorElement.AttachNotify(notify, member));
            }
            else
            {
                elements.Add(InspectorElement.AttachPoll(instance, member, tickProvider));
            }
        }
        return new()
        {
            Elements = elements
        };
    }
}
