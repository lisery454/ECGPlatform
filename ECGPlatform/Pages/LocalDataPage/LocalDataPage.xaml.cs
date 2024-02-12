namespace ECGPlatform;

public partial class LocalDataPage
{
    public LocalDataPage()
    {
        InitializeComponent();
    }

    private void ScrollViewer_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Handled) return;
        var scv = (ScrollViewer)sender;
        scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
        e.Handled = true;
    }
}