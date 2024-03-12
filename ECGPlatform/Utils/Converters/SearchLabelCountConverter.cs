namespace ECGPlatform;

public class SearchLabelCountConverter : IMultiValueConverter
{
    public static readonly SearchLabelCountConverter Instance = new();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2) throw new InvalidOperationException();

        var currentCount = (int)values[0];
        var totalCount = (int)values[1];

        return $"{currentCount} / {totalCount}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}