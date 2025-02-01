using System.Collections;

namespace LgkProductions.Inspector.MetaData;

public sealed partial class MetaDataCollection : IEnumerable<InspectorMember>
{
    readonly IEnumerable<InspectorMember> _members;
    internal MetaDataCollection(IEnumerable<InspectorMember> members)
        => _members = members;

    public IEnumerator<InspectorMember> GetEnumerator()
        => _members.OrderBy(x => x.Order ?? 0).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
