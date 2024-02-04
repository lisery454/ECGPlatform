namespace ECGPlatform;

public class IconHelper
{
    public static PathGeometry GetIcon(DependencyObject obj)
    {
        return (PathGeometry)obj.GetValue(IconProperty);
    }

    public static void SetIcon(DependencyObject obj, PathGeometry value)
    {
        obj.SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached(
        "Icon",
        typeof(PathGeometry),
        typeof(IconHelper),
        new PropertyMetadata()
    );
}