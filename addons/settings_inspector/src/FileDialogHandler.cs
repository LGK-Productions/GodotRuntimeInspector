using System;
using System.Threading.Tasks;
using Godot;
using Microsoft.Extensions.Logging;

namespace SettingInspector.addons.settings_inspector.src;

public static class FileDialogHandler
{
    public static FileDialogHandle CreateNative()
    {
        var fileDialog = new FileDialog();
        fileDialog.FileMode = FileDialog.FileModeEnum.OpenAny;
        fileDialog.Access = FileDialog.AccessEnum.Filesystem;
        fileDialog.CurrentDir = OS.HasFeature("editor")
            ? ProjectSettings.GlobalizePath("res://")
            : OS.GetExecutablePath().GetBaseDir();
        fileDialog.RootSubfolder = String.Empty;
        fileDialog.UseNativeDialog = true;

        return new FileDialogHandle(fileDialog);
    }

    public class FileDialogHandle
    {
        public FileDialog FileDialog { get; }

        private TaskCompletionSource<string> _fileSelectedTcs = new TaskCompletionSource<string>();
        private TaskCompletionSource<string> _dirSelectedTcs = new TaskCompletionSource<string>();
        private TaskCompletionSource<string> _pathSelected = new TaskCompletionSource<string>();
        public event Action<string>? FileSelected;
        public event Action<string>? DirectorySelected;

        public FileDialogHandle(FileDialog fileDialog)
        {
            FileDialog = fileDialog;
            fileDialog.FileSelected += OnFileSelected;
            fileDialog.DirSelected += OnDirectorySelected;
        }

        public async Task<string> WaitForFileSelectedAsync()
        {
            FileDialog.PopupCentered();
            return await _fileSelectedTcs.Task;
        }

        public async Task<string> WaitForDirSelectedAsync()
        {
            FileDialog.PopupCentered();
            return await _dirSelectedTcs.Task;
        }

        public async Task<string> WaitForPathSelectedAsync()
        {
            FileDialog.PopupCentered();
            return await _pathSelected.Task;
        }

        private void OnFileSelected(string path)
        {
            MemberInspectorHandler.Logger?.LogInformation("File selected: {filePath}", path);
            _fileSelectedTcs.TrySetResult(path);
            _pathSelected.TrySetResult(path);
            FileSelected?.Invoke(path);
        }

        private void OnDirectorySelected(string path)
        {
            _dirSelectedTcs.TrySetResult(path);
            _pathSelected.TrySetResult(path);
            DirectorySelected?.Invoke(path);
        }
    }
}
