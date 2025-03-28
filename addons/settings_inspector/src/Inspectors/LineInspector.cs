using Godot;
using LgkProductions.Inspector.MetaData;
using SettingInspector.addons.settings_inspector.src.Attributes;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class LineInspector : MemberInspector
{
	private FileDialog _fileDialog = FileDialogHandler.CreateNative();
	[Export] private Button _filePathButton;
	[Export] private LineEdit _lineEdit;

	protected override void OnInitialize()
	{
		_lineEdit.TextChanged += OnTextChanged;
		_filePathButton.Pressed += OnFilePathButtonPressed;
	}

	protected override void OnRemove()
	{
		_lineEdit.TextChanged -= OnTextChanged;
		_filePathButton.Pressed -= OnFilePathButtonPressed;
	}

	private void OnFilePathButtonPressed()
	{
		_fileDialog.FileMode = FileDialog.FileModeEnum.OpenAny;

		if (FileDialogHandler.Popup(_fileDialog, out var path)) SetValue(path);
	}

	private void OnTextChanged(string newValue)
	{
		OnValueChanged();
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
			member.CustomMetaData.TryGetValue(FilePathAttribute.MetadataKey, out var value) && value is bool and true;
	}

	protected override object? GetValue()
	{
		return _lineEdit.Text;
	}

	public override void SetEditable(bool editable)
	{
		base.SetEditable(editable);
		_lineEdit.Editable = editable;
	}
}
