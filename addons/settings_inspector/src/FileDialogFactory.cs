using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public static class FileDialogFactory
{
    public static FileDialog CreateNativeJson()
    {
        var fileDialog = new FileDialog();
        fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        fileDialog.Access = FileDialog.AccessEnum.Filesystem;
        fileDialog.CurrentDir = ProjectSettings.GlobalizePath("res://");
        fileDialog.UseNativeDialog = true;
        fileDialog.Filters = ["*.json"];

        return fileDialog;
    }
}