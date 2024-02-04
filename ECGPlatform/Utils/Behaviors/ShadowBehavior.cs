using System.Windows.Media.Effects;

namespace ECGPlatform;

public class ShadowBehavior : Behavior<FrameworkElement>
{
    public Brush? Background
    {
        get => (Brush)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        nameof(Background),
        typeof(Brush),
        typeof(ShadowBehavior),
        new PropertyMetadata(null)
    );

    protected override void OnAttached()
    {
        CheckForShadow();
    }

    private void CheckForShadow()
    {
        var objectParent = AssociatedObject.Parent;
        if (objectParent is Grid parent)
        {
            var index = parent.Children.IndexOf(AssociatedObject);
            var copyElement = DeepCopy(AssociatedObject);
            copyElement.Effect = new DropShadowEffect
                { BlurRadius = 10, Direction = 270, Opacity = 0.2, ShadowDepth = 0, Color = Colors.Black };
            if (copyElement is Grid grid)
            {
                grid.Children.Clear();
                if (Background != null)
                    grid.Background = Background;
            }
            else if (copyElement is Border border)
            {
                border.Child = null;
                if (Background != null)
                    border.Background = Background;
            }
            else throw new ArgumentException("Shadow Behavior 不能挂载到Grid和Border以外的组件上");

            parent.Children.Insert(index, copyElement);
        }
    }

    private static FrameworkElement DeepCopy(FrameworkElement element)
    {
        var xaml = XamlWriter.Save(element);
        var xamlString = new StringReader(xaml);
        var xmlTextReader = new XmlTextReader(xamlString);
        var deepCopyObject = (FrameworkElement)XamlReader.Load(xmlTextReader);
        return deepCopyObject;
    }
}