namespace ECGPlatform;

public class RPeakLabelToStrConverter : IValueConverter
{
    public static readonly RPeakLabelToStrConverter Instance = new();

    public string Convert(RPeakLabel value)
    {
        return (string)Convert(value,
            typeof(string),
            null, CultureInfo.CurrentCulture);
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return "null";
        return (RPeakLabel)value switch
        {
            RPeakLabel.NONE => GetStr("RPeakLabel.None"),
            RPeakLabel.SINUS_RHYTHM => GetStr("RPeakLabel.SinusRhythm"),
            RPeakLabel.VENTRICULAR_PREEXCITATION => GetStr("RPeakLabel.VentricularPreexcitation"),
            RPeakLabel.PREMATURE_ATRIAL_CONTRACTIONS => GetStr("RPeakLabel.PrematureAtrialContractions"),
            RPeakLabel.PREMATURE_VENTRICULAR_CONTRACTIONS => GetStr("RPeakLabel.PrematureVentricularContractions"),
            RPeakLabel.ATRIAL_FIBRILLATION => GetStr("RPeakLabel.AtrialFibrillation"),
            RPeakLabel.ATRIAL_FLUTTER => GetStr("RPeakLabel.AtrialFlutter"),
            RPeakLabel.VENTRICULAR_FLUTTER_VENTRICULAR_FIBRILLATION => GetStr(
                "RPeakLabel.VentricularFlutterVentricularFibrillation"),
            RPeakLabel.ATRIOVENTRICULAR_BLOCK => GetStr("RPeakLabel.AtrioventricularBlock"),
            RPeakLabel.NOISE => GetStr("RPeakLabel.Noise"),
            RPeakLabel.PAROXYSMAL_SUPRAVENTRICULAR_TACHYCARDIA => GetStr(
                "RPeakLabel.ParoxysmalSupraventricularTachycardia"),
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