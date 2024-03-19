namespace ECGPlatform;

public class MarkIntervalLabelToStrConverter: IValueConverter
{
    public static readonly MarkIntervalLabelToStrConverter Instance = new();

    public string Convert(MarkIntervalLabel value)
    {
        return (string)Convert(value,
            typeof(string),
            null, CultureInfo.CurrentCulture);
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return "null";
        return (MarkIntervalLabel)value switch
        {
            MarkIntervalLabel.ATRIAL_FIBRILLATION => GetStr("MarkIntervalLabel.AtrialFibrillation"),
            MarkIntervalLabel.NOISE => GetStr("MarkIntervalLabel.Noise"),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    private static string GetStr(string key)
    {
        return (string)App.Current.FindResource(key)!;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}