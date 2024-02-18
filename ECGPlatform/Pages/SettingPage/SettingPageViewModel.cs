namespace ECGPlatform;

public partial class SettingPageViewModel : ObservableObject
{
    private readonly ISettingManager _settingManager;
    private readonly ILogger _logger;

    public SettingPageViewModel(ISettingManager settingManager, ILogger logger)
    {
        _settingManager = settingManager;
        _localDataDirectoryPath = settingManager.CurrentSetting.LocalDataDirectoryPath;
        _logger = logger;
        logger.Information("Setting Page ViewModel Create.");
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