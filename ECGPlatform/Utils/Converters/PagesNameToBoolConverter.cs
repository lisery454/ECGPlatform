namespace ECGPlatform;

public class PagesNameToBoolConverter : IValueConverter
{
    public static PagesNameToBoolConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (PagesName)value == (PagesName)parameter;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? parameter : throw new Exception("没有传参，PagesName");
    }
}