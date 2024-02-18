namespace ECGPlatform;

public partial class LocalDataPage
{
    public LocalDataPage()
    {
        InitializeComponent();
    }

    private void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Debug.WriteLine("Click");
    }
}