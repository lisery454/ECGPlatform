namespace ECGPlatform;

[ValueConversion(typeof(float), typeof(float))]
internal class BindingSumConverter : IMultiValueConverter
{
    public static BindingSumConverter Instance = new BindingSumConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var result = 0f;
        foreach (var item in values)
        {
            var f = (float)item;
            result += f;
        }
        return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
