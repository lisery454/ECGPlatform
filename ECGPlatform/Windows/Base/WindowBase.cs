using System.Windows.Shell;

namespace ECGPlatform;

public class WindowBase : Window
{
    protected WindowBase()
    {
        Loaded += Window_Loaded;
        SourceInitialized += Window_SourceInitialized;
        WindowCornerRestorer.ApplyRoundCorner(this);
        SetWindowChrome();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowAnimRestorer.AddAnimTo(this);
    }

    void Window_SourceInitialized(object? sender, EventArgs e)
    {
        // WindowResizer.CoverResizeIssue(this);
    }

    private void SetWindowChrome()
    {
        var captionHeight = (double)Application.Current.FindResource("CaptionHeight")!;
        var resizeBorderSize = (double)Application.Current.FindResource("ResizeBorderSize")!;
        var windowChrome = new WindowChrome
        {
            CaptionHeight = captionHeight, GlassFrameThickness = new Thickness(0),
            ResizeBorderThickness = new Thickness(resizeBorderSize)
        };
        var binding = new Binding
        {
            Mode = BindingMode.OneWay, Source = this, Path = new PropertyPath("WindowState"),
            Converter = WindowStateToCaptionHeightConverter.Instance
        };
        BindingOperations.SetBinding(windowChrome, WindowChrome.CaptionHeightProperty, binding);
        SetValue(WindowChrome.WindowChromeProperty, windowChrome);
    }
}