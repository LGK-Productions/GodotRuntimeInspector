using Godot;
using LgkProductions.Inspector.MetaData;
using SettingInspector.Attributes;
using SettingInspector.Util;

namespace SettingInspector.Inspectors.Primitives;

public partial class ToggleInspector : MemberInspector
{
    [Export] private Button? _checkbox;

    private Button? _resolvedButton;
    [Export] private Button? _toggle;

    protected override void OnInitialize()
    {
        _resolvedButton = _toggle;
        _checkbox!.Pressed += OnPressed;
        _toggle!.Pressed += OnPressed;
    }

    protected override void OnRemove()
    {
        _checkbox!.Pressed -= OnPressed;
        _toggle!.Pressed -= OnPressed;
    }

    protected override void SetValueInternal(object value)
    {
        _resolvedButton!.SetPressed((bool)value);
    }

    protected override object GetValue()
    {
        return _resolvedButton!.IsPressed();
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        _resolvedButton!.Disabled = !editable;
    }

    protected override void OnSetMetaData(MetaDataMember member)
    {
        _checkbox!.Visible = false;
        _toggle!.Visible = false;
        if (member.TryGetMetaData(CheckboxAttribute.MetadataKey, out var res) && res)
            _resolvedButton = _checkbox;
        else
            _resolvedButton = _toggle;

        _resolvedButton.Visible = true;
        base.OnSetMetaData(member);
    }

    private void OnPressed()
    {
        OnValueChanged(new ValueChangeTree(this, _resolvedButton!.IsPressed()));
    }
}