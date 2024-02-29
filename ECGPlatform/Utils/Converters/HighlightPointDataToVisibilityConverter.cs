namespace ECGPlatform;

public class HighlightPointDataToVisibilityConverter : IValueConverter
{
    public static readonly HighlightPointDataToVisibilityConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var highlightPointData = (value as HighlightPointData);
        if (highlightPointData == null) return Visibility.Collapsed;
        if (parameter == null) return Visibility.Collapsed;
        var pointType = (PointType)parameter;
        return pointType == highlightPointData.PointType ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}