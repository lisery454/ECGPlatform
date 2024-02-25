namespace ECGPlatform;

[ValueConversion(typeof(WindowState), typeof(Thickness))]
public class WindowStateToWindowBorderMarginConverter : IValueConverter
{
    public static WindowStateToWindowBorderMarginConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var resizeBorderSize = (double)Application.Current.FindResource("ResizeBorderSize")!;

        var state = (WindowState)value;
        return state switch
        {
            WindowState.Normal => new Thickness(0),
            WindowState.Minimized => new Thickness(0),
            WindowState.Maximized => new Thickness(resizeBorderSize),
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}
