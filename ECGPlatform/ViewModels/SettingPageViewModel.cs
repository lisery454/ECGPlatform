namespace ECGPlatform;

public partial class SettingPageViewModel : ObservableObject
{
    private readonly ISettingManager _settingManager;
    private readonly ILogger _logger;
    private readonly ILanguageManager _languageManager;

    public SettingPageViewModel(ISettingManager settingManager, ILogger logger, ILanguageManager languageManager)
    {
        _settingManager = settingManager;
        _localDataDirectoryPath = settingManager.CurrentSetting.LocalDataDirectoryPath;
        _languageManager = languageManager;
        _isChineseChecked = settingManager.CurrentSetting.LanguageType == LanguageType.CHINESE;
        _isEnglishChecked = settingManager.CurrentSetting.LanguageType == LanguageType.ENGLISH;
        _logger = logger;
        logger.Information("Setting Page ViewModel Create.");
    }

    [ObservableProperty] private string _localDataDirectoryPath;
    [ObservableProperty] private bool _isChineseChecked;
    [ObservableProperty] private bool _isEnglishChecked;

    [RelayCommand]
    private void ChangeLocalDataDirectoryPath()
    {
        var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            LocalDataDirectoryPath = dialog.FileName;
            _settingManager.CurrentSetting.LocalDataDirectoryPath = LocalDataDirectoryPath;
            _settingManager.Save();
            _logger.Information($"Change LocalDataDirectoryPath {LocalDataDirectoryPath} Success.");
        }
    }

    [RelayCommand]
    private void ChangeLanguage(LanguageType languageType)
    {
        _languageManager.ChangeLanguage(languageType);
        _settingManager.CurrentSetting.LanguageType = languageType;
        _settingManager.Save();
        _logger.Information($"Change Language {languageType} Success.");
    }
}