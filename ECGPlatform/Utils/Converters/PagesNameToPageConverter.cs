namespace ECGPlatform;

public class PagesNameToPageConverter : IValueConverter
{
    public static PagesNameToPageConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var pagesName = (PagesName)value;
        return pagesName switch
        {
            PagesName.LOCAL_DATA_PAGE => App.Current.Services.GetService<LocalDataPage>()!,
            PagesName.SETTING_PAGE => App.Current.Services.GetService<SettingPage>()!,
            PagesName.REMOTE_PAGE => App.Current.Services.GetService<RemotePage>()!,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}