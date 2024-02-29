namespace ECGPlatform;

public class SolidColorAnimation : ColorAnimation
{
    public SolidColorBrush? ToBrush
    {
        get => (SolidColorBrush?)GetValue(ToBrushProperty);
        set
        {
            SetValue(ToBrushProperty, value);
            To = value?.Color;
        }
    }

    public static readonly DependencyProperty ToBrushProperty = DependencyProperty.Register(
        nameof(ToBrush),
        typeof(SolidColorBrush),
        typeof(SolidColorAnimation),
        new PropertyMetadata(null)
    );
}