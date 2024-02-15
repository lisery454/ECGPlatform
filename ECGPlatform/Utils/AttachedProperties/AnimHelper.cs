namespace ECGPlatform;

public class AnimHelper
{
    public static Storyboard GetAnim0(DependencyObject obj)
    {
        return (Storyboard)obj.GetValue(Anim0Property);
    }

    public static void SetAnim0(DependencyObject obj, Storyboard value)
    {
        obj.SetValue(Anim0Property, value);
    }

    public static readonly DependencyProperty Anim0Property = DependencyProperty.RegisterAttached(
        "Anim0",
        typeof(Storyboard),
        typeof(AnimHelper),
        new PropertyMetadata()
    );

    public static Storyboard GetAnim1(DependencyObject obj)
    {
        return (Storyboard)obj.GetValue(Anim1Property);
    }

    public static void SetAnim1(DependencyObject obj, Storyboard value)
    {
        obj.SetValue(Anim1Property, value);
    }

    public static readonly DependencyProperty Anim1Property = DependencyProperty.RegisterAttached(
        "Anim1",
        typeof(Storyboard),
        typeof(AnimHelper),
        new PropertyMetadata()
    );
}