using System;
using System.Collections.Generic;
using LgkProductions.Inspector;

namespace SettingInspector.addons.settings_inspector.src.InspectorCollections;

public interface IMemberInspectorCollection : IEnumerable<(InspectorElement, MemberInspector)>
{
    public void SetMemberInspector(Inspector inspector);

    public void AddElement(InspectorElement element);

    public void WriteBack();

    public void Clear();

    public void SetEditable(bool editable);

    public void SetScrollable(bool scrollable);
    
    public event Action ValueChanged;
}