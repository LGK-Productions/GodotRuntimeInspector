using System;
using System.Collections.Generic;
using LgkProductions.Inspector;
using RuntimeInspector.Util;

namespace RuntimeInspector.Inspectors.Collections;

public interface IMemberInspectorCollection : IEnumerable<(InspectorElement, MemberInspector)>
{
    public void SetMemberInspector(Inspector inspector);

    public void AddElement(InspectorElement element);

    public void WriteBack();

    public void Remove();

    public void SetEditable(bool editable);

    public void SetScrollable(bool scrollable);

    public event Action<ValueChangeTree>? ValueChanged;
}