namespace ECGPlatform;

public class ScrollViewerMouseWheelBehavior : Behavior<ScrollViewer>
{
    protected override void OnAttached()
    {
        AssociatedObject.PreviewMouseWheel +=
            (sender, e) =>
            {
                if (e.Handled) return;
                var scv = (ScrollViewer)sender;
                scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
                e.Handled = true;
            };
    }
}