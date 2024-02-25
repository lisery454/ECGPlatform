namespace ECGPlatform;

[ValueConversion(typeof(WindowState), typeof(PathGeometry))]
public class WindowStateToMaximizeButtonIconConverter : IValueConverter
{
    public static WindowStateToMaximizeButtonIconConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var maximizeIcon = (PathGeometry)Application.Current.FindResource("MaximizeIcon")!;
        var maximize2Icon = (PathGeometry)Application.Current.FindResource("Maximize2Icon")!;

        var state = (WindowState)value;
        return state switch
        {
            WindowState.Normal => maximizeIcon,
            WindowState.Minimized => maximizeIcon,
            WindowState.Maximized => maximize2Icon,
            _ => throw new ArgumentOutOfRangeException(nameof(state)),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}
