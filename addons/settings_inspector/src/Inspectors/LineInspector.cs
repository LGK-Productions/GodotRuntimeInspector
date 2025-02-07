using Godot;
using LgkProductions.Inspector.MetaData;

namespace SettingInspector.addons.settings_inspector.src.Inspectors;

public partial class LineInspector : MemberInspector
{
	[Export] private LineEdit _lineEdit;
	[Export] private Button _filePathButton;
	
	private FileDialog _fileDialog = FileDialogHandler.CreateNative();

	public override void _EnterTree()
	{
		base._EnterTree();
		_lineEdit.TextChanged += OnTextChanged;
		_filePathButton.Pressed += OnFilePathButtonPressed;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_lineEdit.TextChanged -= OnTextChanged;
		_filePathButton.Pressed -= OnFilePathButtonPressed;
	}

	private void OnFilePathButtonPressed()
	{
		_fileDialog.FileMode = FileDialog.FileModeEnum.OpenAny;

		if (FileDialogHandler.Popup(_fileDialog, out string path))
		{
			SetValue(path);
		}
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
			member.CustomMetaData.TryGetValue("FilePath", out var value) && value is bool and true;
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
