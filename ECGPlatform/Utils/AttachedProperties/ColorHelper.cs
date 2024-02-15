namespace ECGPlatform;

public class ColorHelper
{
    public static Color GetColor0(DependencyObject obj)
    {
        return (Color)obj.GetValue(Color0Property);
    }

    public static void SetColor0(DependencyObject obj, Color value)
    {
        obj.SetValue(Color0Property, value);
    }

    public static readonly DependencyProperty Color0Property = DependencyProperty.RegisterAttached(
        "Color0",
        typeof(Color),
        typeof(ColorHelper),
        new PropertyMetadata(Colors.Red)
    );
    
    public static Color GetColor1(DependencyObject obj)
    {
        return (Color)obj.GetValue(Color1Property);
    }

    public static void SetColor1(DependencyObject obj, Color value)
    {
        obj.SetValue(Color1Property, value);
    }

    public static readonly DependencyProperty Color1Property = DependencyProperty.RegisterAttached(
        "Color1",
        typeof(Color),
        typeof(ColorHelper),
        new PropertyMetadata(Colors.Red)
    );
    
    public static Color GetColor2(DependencyObject obj)
    {
        return (Color)obj.GetValue(Color1Property);
    }

    public static void SetColor2(DependencyObject obj, Color value)
    {
        obj.SetValue(Color2Property, value);
    }

    public static readonly DependencyProperty Color2Property = DependencyProperty.RegisterAttached(
        "Color2",
        typeof(Color),
        typeof(ColorHelper),
        new PropertyMetadata(Colors.Red)
    );
    
    public static Color GetColor3(DependencyObject obj)
    {
        return (Color)obj.GetValue(Color3Property);
    }

    public static void SetColor3(DependencyObject obj, Color value)
    {
        obj.SetValue(Color3Property, value);
    }

    public static readonly DependencyProperty Color3Property = DependencyProperty.RegisterAttached(
        "Color3",
        typeof(Color),
        typeof(ColorHelper),
        new PropertyMetadata(Colors.Red)
    );
    
    public static Color GetColor4(DependencyObject obj)
    {
        return (Color)obj.GetValue(Color4Property);
    }

    public static void SetColor4(DependencyObject obj, Color value)
    {
        obj.SetValue(Color4Property, value);
    }

    public static readonly DependencyProperty Color4Property = DependencyProperty.RegisterAttached(
        "Color4",
        typeof(Color),
        typeof(ColorHelper),
        new PropertyMetadata(Colors.Red)
    );
    
    public static Color GetColor6(DependencyObject obj)
    {
        return (Color)obj.GetValue(Color6Property);
    }

    public static void SetColor6(DependencyObject obj, Color value)
    {
        obj.SetValue(Color6Property, value);
    }

    public static readonly DependencyProperty Color6Property = DependencyProperty.RegisterAttached(
        "Color6",
        typeof(Color),
        typeof(ColorHelper),
        new PropertyMetadata(Colors.Red)
    );
}