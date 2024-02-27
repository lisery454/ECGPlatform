namespace ECGPlatform;

public class ThemeManager : IThemeManager
{
    public ThemeManager(ISettingManager settingManager)
    {
        ChangeTheme(settingManager.CurrentSetting.ThemeType);
    }


    public void ChangeTheme(ThemeType themeType)
    {
        var themeFilePath = themeType switch
        {
            ThemeType.LIGHT => "Resources/Colors.xaml",
            ThemeType.DARK => "Resources/ColorsDark.xaml",
            _ => throw new ArgumentOutOfRangeException(nameof(themeType), themeType, null)
        };

        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri(themeFilePath, UriKind.Relative)
        };

        Application.Current.Resources.MergedDictionaries[2] = resourceDictionary;
    }
}