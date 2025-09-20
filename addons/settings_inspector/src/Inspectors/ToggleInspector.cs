using Godot;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.Attributes;
using SettingInspector.addons.settings_inspector.src.ValueTree;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class ToggleInspector : MemberInspector
{
    [Export] private Button _checkbox;
    [Export] private Button _toggle;

    private Button _resolvedButton;

    protected override void OnInitialize()
    {
        _resolvedButton = _toggle;
        _checkbox.Pressed += OnPressed;
        _toggle.Pressed += OnPressed;
    }

    protected override void OnRemove()
    {
        _checkbox.Pressed -= OnPressed;
        _toggle.Pressed -= OnPressed;
    }

    protected override void SetValue(object value)
    {
        base.SetValue(value);
        _resolvedButton.SetPressed((bool)value);
    }

    protected override object? GetValue()
    {
        return _resolvedButton.IsPressed();
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        _resolvedButton.Disabled = !editable;
    }

    protected override void OnSetMetaData(MetaDataMember member)
    {
        _checkbox.Visible = false;
        _toggle.Visible = false;
        if (member.TryGetMetaData(CheckboxAttribute.MetadataKey, out var res) && res)
        {
            _resolvedButton = _checkbox;
        }
        else
        {
            _resolvedButton = _toggle;
        }

        _resolvedButton.Visible = true;
        base.OnSetMetaData(member);
    }

    private void OnPressed()
    {
        OnValueChanged(new ValueChangeTree(this, _resolvedButton.IsPressed()));
    }
}
