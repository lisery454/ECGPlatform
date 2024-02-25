namespace ECGPlatform;

[ValueConversion(typeof(WindowState), typeof(double))]
public class WindowStateToCaptionHeightConverter : IValueConverter
{
    public static WindowStateToCaptionHeightConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var captionHeight = (double)Application.Current.FindResource("CaptionHeight")!;
        var resizeBorderSize = (double)Application.Current.FindResource("ResizeBorderSize")!;
        var state = (WindowState)value;
        return state switch
        {
            WindowState.Normal => captionHeight,
            WindowState.Minimized => captionHeight,
            WindowState.Maximized => captionHeight + resizeBorderSize,
            _ => throw new ArgumentOutOfRangeException(nameof(state)),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}