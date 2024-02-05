namespace ECGPlatform;

public class PagesNameToPageConverter : IValueConverter
{
    public static PagesNameToPageConverter Instance { get; } = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var pagesName = (PagesName)value;
        return pagesName switch
        {
            PagesName.LocalDataPage => new LocalDataPage { DataContext = new LocalDataPage() },
            PagesName.SettingPage => new SettingPage { DataContext = new SettingPage() },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}