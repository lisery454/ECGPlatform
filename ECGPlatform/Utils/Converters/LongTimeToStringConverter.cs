namespace ECGPlatform;

public class LongTimeToStringConverter: IValueConverter
{
    public static LongTimeToStringConverter Instance = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var time = (long)value;
        return TimeFormatter.MircoSecondsToString(time);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}