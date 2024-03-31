namespace ECGPlatform;

public class XGridValueConverter : IValueConverter
{
    public static XGridValueConverter Instance => new();
    
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString();
    }

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        var tryParse = float.TryParse((string)value!, out var res);
        if (!tryParse) throw new Exception("不能转化字符串为float");
        return res;
    }
}