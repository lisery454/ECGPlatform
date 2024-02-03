namespace ECGPlatform;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowCornerRestorer.ApplyRoundCorner(this);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowAnimRestorer.AddAnimTo(this);
    }
}
