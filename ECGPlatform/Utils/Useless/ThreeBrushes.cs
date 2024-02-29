namespace ECGPlatform;

public class ThreeBrushes: DependencyObject
{
    public SolidColorBrush Brush0
    {
        get =>  (SolidColorBrush) GetValue(Brush0Property);
        set => SetValue(Brush0Property, value);
    }

    public static readonly DependencyProperty Brush0Property = DependencyProperty.Register(
        nameof(Brush0),
        typeof(SolidColorBrush),
        typeof(ThreeBrushes),
        new PropertyMetadata(null)
    );
    
    public SolidColorBrush Brush1
    {
        get =>  (SolidColorBrush) GetValue(Brush1Property);
        set => SetValue(Brush1Property, value);
    }

    public static readonly DependencyProperty Brush1Property = DependencyProperty.Register(
        nameof(Brush1),
        typeof(SolidColorBrush),
        typeof(ThreeBrushes),
        new PropertyMetadata(null)
    );
    
    public SolidColorBrush Brush2
    {
        get =>  (SolidColorBrush) GetValue(Brush2Property);
        set => SetValue(Brush2Property, value);
    }

    public static readonly DependencyProperty Brush2Property = DependencyProperty.Register(
        nameof(Brush2),
        typeof(SolidColorBrush),
        typeof(ThreeBrushes),
        new PropertyMetadata(null)
    );
}