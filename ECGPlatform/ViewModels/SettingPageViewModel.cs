namespace ECGPlatform;

public partial class SettingPageViewModel : ObservableObject
{
    private readonly ISettingManager _settingManager;
    private readonly ILogger _logger;
    private readonly ILanguageManager _languageManager;
    private readonly IThemeManager _themeManager;

    public SettingPageViewModel(ISettingManager settingManager, ILogger logger, ILanguageManager languageManager,
        IThemeManager themeManager)
    {
        _settingManager = settingManager;
        _localDataDirectoryPath = settingManager.CurrentSetting.LocalDataDirectoryPath;
        _languageManager = languageManager;
        _themeManager = themeManager;
        _isChineseChecked = settingManager.CurrentSetting.LanguageType == LanguageType.CHINESE;
        _isEnglishChecked = settingManager.CurrentSetting.LanguageType == LanguageType.ENGLISH;
        _isLightThemeChecked = settingManager.CurrentSetting.ThemeType == ThemeType.LIGHT;
        _isDarkThemeChecked = settingManager.CurrentSetting.ThemeType == ThemeType.DARK;
        _logger = logger;
        logger.Information("Setting Page ViewModel Create.");
    }

    [ObservableProperty] private string _localDataDirectoryPath;
    [ObservableProperty] private bool _isChineseChecked;
    [ObservableProperty] private bool _isEnglishChecked;
    [ObservableProperty] private bool _isLightThemeChecked;
    [ObservableProperty] private bool _isDarkThemeChecked;

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

    [RelayCommand]
    private void ChangeTheme(ThemeType themeType)
    {
        _themeManager.ChangeTheme(themeType);
        _settingManager.CurrentSetting.ThemeType = themeType;
        _settingManager.Save();
        _logger.Information($"Change Theme {themeType} Success.");
    }
}