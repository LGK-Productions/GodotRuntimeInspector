using Godot;

namespace SettingInspector.addons.settings_inspector.src;

public static class FileDialogHandler
{
    public static FileDialog CreateNative()
    {
        var fileDialog = new FileDialog();
        fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        fileDialog.Access = FileDialog.AccessEnum.Filesystem;
        fileDialog.CurrentDir = ProjectSettings.GlobalizePath("res://");
        fileDialog.UseNativeDialog = true;

        return fileDialog;
    }

    public static bool Popup(FileDialog fileDialog, out string path)
    {
        path = string.Empty;
        string? result = null;
        fileDialog.FileSelected += SetPath;
        fileDialog.Popup();
        fileDialog.FileSelected -= SetPath;
        if (result == null) return false;
        path = result;
        return true;

        void SetPath(string val)
        {
            result = val;
        }
    }
}