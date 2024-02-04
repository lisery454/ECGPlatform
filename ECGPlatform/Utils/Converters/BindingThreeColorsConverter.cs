namespace ECGPlatform;

public class BindingThreeColorsConverter : IMultiValueConverter
{
    public static BindingThreeColorsConverter Instance = new();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var threeBrushes = (ThreeBrushes)parameter;
        var index0 = (double)values[0];
        var index1 = (double)values[1];
        var index2 = (double)values[2];

        var color0 = Color.Multiply(threeBrushes.Brush0.Color, (float)index0);
        var color1 = Color.Multiply(threeBrushes.Brush1.Color, (float)index1);
        var color2 = Color.Multiply(threeBrushes.Brush2.Color, (float)index2);
        var finalColor = color0 + color1 + color2;
        return new SolidColorBrush(finalColor);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}