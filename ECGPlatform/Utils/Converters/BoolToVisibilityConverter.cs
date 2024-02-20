namespace ECGPlatform;

public class BoolToVisibilityConverter : IValueConverter
{
    public static BoolToVisibilityConverter InstanceHiddenTrueForVisible { get; } = new(Visibility.Hidden, false);
    public static BoolToVisibilityConverter InstanceHiddenFalseForVisible { get; } = new(Visibility.Hidden, true);
    public static BoolToVisibilityConverter InstanceCollapsedTrueForVisible { get; } = new(Visibility.Collapsed, false);
    public static BoolToVisibilityConverter InstanceCollapsedFalseForVisible { get; } = new(Visibility.Collapsed, true);

    private readonly Visibility _falseVisibility;
    private readonly bool _isReverse;

    public BoolToVisibilityConverter(Visibility falseVisibility, bool isReverse)
    {
        _falseVisibility = falseVisibility;
        _isReverse = isReverse;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return _isReverse switch
        {
            true => (bool)value switch
            {
                false => Visibility.Visible,
                true => _falseVisibility,
            },
            false => (bool)value switch
            {
                true => Visibility.Visible,
                false => _falseVisibility,
            },
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}