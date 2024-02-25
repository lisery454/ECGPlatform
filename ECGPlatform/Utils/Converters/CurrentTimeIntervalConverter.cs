namespace ECGPlatform;

public class CurrentTimeIntervalConverter : IMultiValueConverter
     {
         public static readonly CurrentTimeIntervalConverter Instance = new();
     
         public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
         {
             var currentTime = (long)values[0];
             var timeInterval = (long)values[1];
     
             return TimeFormatter.MircoSecondsToString(currentTime + timeInterval);
         }
     
         public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
         {
             throw new InvalidOperationException();
         }
}