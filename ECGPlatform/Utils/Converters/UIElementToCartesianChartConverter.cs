namespace ECGPlatform;

public class UIElementToCartesianChartConverter : IValueConverter
{
    public static UIElementToCartesianChartConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (CartesianChart)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}