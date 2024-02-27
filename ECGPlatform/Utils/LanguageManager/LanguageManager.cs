namespace ECGPlatform;

public class LanguageManager : ILanguageManager
{
    public LanguageManager(ISettingManager settingManager)
    {
        ChangeLanguage(settingManager.CurrentSetting.LanguageType);
    }

    public void ChangeLanguage(LanguageType languageType)
    {
        var languageFilePath = languageType switch
        {
            LanguageType.CHINESE => "Resources/Strings/Strings_zh.xaml",
            LanguageType.ENGLISH => "Resources/Strings/Strings_en.xaml",
            _ => throw new ArgumentOutOfRangeException(nameof(languageType), languageType, null)
        };

        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri(languageFilePath, UriKind.Relative)
        };

        Application.Current.Resources.MergedDictionaries[1] = resourceDictionary;
    }
}