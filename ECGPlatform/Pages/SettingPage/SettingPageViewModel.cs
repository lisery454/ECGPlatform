namespace ECGPlatform;

public partial class SettingPageViewModel : ObservableObject
{
    private readonly ISettingManager _settingManager;

    public SettingPageViewModel(ISettingManager settingManager)
    {
        _settingManager = settingManager;
        _localDataDirectoryPath = settingManager.CurrentSetting.LocalDataDirectoryPath;
    }

    [ObservableProperty] private string _localDataDirectoryPath;

    [RelayCommand]
    private void ChangeLocalDataDirectoryPath()
    {
        var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            LocalDataDirectoryPath = dialog.FileName;
            _settingManager.CurrentSetting.LocalDataDirectoryPath = LocalDataDirectoryPath;
            _settingManager.Save();
        }
    }
}