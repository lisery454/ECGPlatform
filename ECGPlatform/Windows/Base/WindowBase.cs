using System.Windows.Shell;

namespace ECGPlatform;

public class WindowBase : Window
{
    public WindowBase()
    {
        RenderTransformOrigin = new Point(0.5, 0.5);
        RenderTransform = new TransformGroup
        {
            Children =
            {
                new ScaleTransform(),
                new TranslateTransform(),
            }
        };

        MouseDown += OnMouseDown;
        Loaded += Window_Loaded;
        // SourceInitialized += Window_SourceInitialized;
        // WindowAnimRestorer.AddAnimTo(this);
        WindowCornerRestorer.ApplyRoundCorner(this);
        SetWindowChrome();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Keyboard.ClearFocus();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowAnimRestorer.AddAnimTo(this);
        // var winBootAnim = (Storyboard)Application.Current.FindResource("WinBootAnimation")!;
        // BeginStoryboard(winBootAnim);
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