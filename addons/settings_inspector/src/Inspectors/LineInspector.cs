using Godot;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.Attributes;
using SettingInspector.addons.settings_inspector.src.ValueTree;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class LineInspector : MemberInspector
{
    private FileDialogHandler.FileDialogHandle _fileDialogHandle = FileDialogHandler.CreateNative();
    [Export] private Button _filePathButton;
    [Export] private LineEdit _lineEdit;

    protected override void OnInitialize()
    {
        _lineEdit.TextChanged += OnTextChanged;
        _filePathButton.Pressed += OnFilePathButtonPressed;
        if (_fileDialogHandle.FileDialog.GetParent() == null)
            AddChild(_fileDialogHandle.FileDialog);
    }

    protected override void OnRemove()
    {
        _lineEdit.TextChanged -= OnTextChanged;
        _filePathButton.Pressed -= OnFilePathButtonPressed;
    }

    private async void OnFilePathButtonPressed()
    {
        SetValue(await _fileDialogHandle.WaitForFileSelectedAsync());
    }

    private void OnTextChanged(string newValue)
    {
        OnValueChanged(new ValueChangeTree(this, newValue));
    }

    protected override void SetValue(object value)
    {
        base.SetValue(value);
        _lineEdit.Text = value.ToString();
    }

    protected override void OnSetMetaData(MetaDataMember member)
    {
        base.OnSetMetaData(member);
        _filePathButton.Visible =
            member.TryGetMetaData(PathPickerAttribute.PickerTypeKey, out var value);
        _fileDialogHandle.FileDialog.FileMode = value;
        if (member.TryGetMetaData(PathPickerAttribute.FilterKey, out var filters))
        {
            _fileDialogHandle.FileDialog.Filters = filters;
        }
    }

    protected override object? GetValue()
    {
        return _lineEdit.Text;
    }

    public override void SetEditable(bool editable)
    {
        base.SetEditable(editable);
        _lineEdit.Editable = editable;
        _filePathButton.Disabled = !editable;
    }
}
