namespace ECGPlatform;

public class TwoMarkIntervalPointsConverter : IMultiValueConverter
{
    public static TwoMarkIntervalPointsConverter Instance = new();
    
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2) throw new InvalidOperationException();
        var point0 = (HighlightPointData)values[0];
        var point1 = (HighlightPointData)values[1];

        if (point0 == null || point1 == null) return string.Empty;

        var timeStr = (string)App.Current.FindResource("TimeStr")!;
        var time0 = TimeFormatter.MircoSecondsToString(point0.Time);
        var time1 = TimeFormatter.MircoSecondsToString(point1.Time);


        return point0.Time < point1.Time ? $"{timeStr}: {time0} - {time1}" : $"{timeStr}: {time1} - {time0}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}