using EventTrigger = System.Windows.EventTrigger;

namespace ECGPlatform;

public class BorderAnimBuildBehavior : Behavior<Border>
{
    public Storyboard? EnterAnim
    {
        get => (Storyboard)GetValue(EnterAnimProperty);
        set => SetValue(EnterAnimProperty, value);
    }

    public static readonly DependencyProperty EnterAnimProperty = DependencyProperty.Register(
        nameof(EnterAnim),
        typeof(Storyboard),
        typeof(BorderAnimBuildBehavior),
        new PropertyMetadata(null)
    );

    public Storyboard? ExitAnim
    {
        get => (Storyboard)GetValue(ExitAnimProperty);
        set => SetValue(ExitAnimProperty, value);
    }

    public static readonly DependencyProperty ExitAnimProperty = DependencyProperty.Register(
        nameof(ExitAnim),
        typeof(Storyboard),
        typeof(BorderAnimBuildBehavior),
        new PropertyMetadata(null)
    );


    protected override void OnAttached()
    {
        var enterAnim = new ColorAnimation(toValue: Colors.Red,
            duration: new Duration(TimeSpan.FromMilliseconds(300)));
        enterAnim
            .SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("Background"));
        EnterAnim = new Storyboard { Children = { enterAnim } };

        var exitAnim = new ColorAnimation(toValue: Colors.Transparent,
            duration: new Duration(TimeSpan.FromMilliseconds(300)));
        exitAnim
            .SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("Background"));
        ExitAnim = new Storyboard { Children = { exitAnim } };

        AssociatedObject.Triggers.Add(new EventTrigger
        {
            RoutedEvent = UIElement.MouseEnterEvent,
            Actions = { new BeginStoryboard { Storyboard = EnterAnim } },
        });
        AssociatedObject.Triggers.Add(new EventTrigger
        {
            RoutedEvent = UIElement.MouseLeaveEvent,
            Actions = { new BeginStoryboard { Storyboard = ExitAnim } },
        });
    }
}