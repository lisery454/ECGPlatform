namespace ECGPlatform;

public class HighlightPointDataToStrConverter : IValueConverter
{
    public static readonly HighlightPointDataToStrConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var highlightPointData = value as HighlightPointData;

        var defaultPointDataInfoStr = (string)App.Current.FindResource("DefaultPointDataInfoStr")!;
        var timeStr = (string)App.Current.FindResource("TimeStr")!;
        var voltageStr = (string)App.Current.FindResource("VoltageStr")!;
        var labelStr = (string)App.Current.FindResource("LabelStr")!;

        if (highlightPointData == null) return defaultPointDataInfoStr;

        var time = TimeFormatter.MircoSecondsToString(highlightPointData.Time);
        var voltage = highlightPointData.Value.ToFormat(f => (float)Math.Round(f, 4));
        var label = highlightPointData.Label;

        return highlightPointData.PointType switch
        {
            PointType.SIMPLE_POINT =>
                $"{timeStr}: {time}; {voltageStr}: {voltage} mV",
            PointType.R_PEAKS_POINT =>
                $"{timeStr}: {time}; {voltageStr}:{voltage} mV; {labelStr}: {label} ",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}