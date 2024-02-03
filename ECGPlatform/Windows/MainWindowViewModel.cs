using System.Windows;

namespace ECGPlatform;

partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _message;
    public float CaptionHeight { get; private set; }
    public float ResizeBorderThickness { get; private set; }

    public float AmendedCaptionHeight => CaptionHeight + ResizeBorderThickness;

    private readonly ILogger _logger;

    public MainWindowViewModel(ILogger logger)
    {
        _logger = logger;
        _message = "Hello, World";
        CaptionHeight = 30f;
        ResizeBorderThickness = 6f;
        _logger.Information($"{typeof(MainWindowViewModel)} Create.");
    }

    [RelayCommand]
    private void Minimize(Window window)
    {
        SystemCommands.MinimizeWindow(window);
    }

    [RelayCommand]
    private void Maximize(Window window)
    {
        if (window.WindowState == WindowState.Normal)
        {
            SystemCommands.MaximizeWindow(window);
        }
        else
        {
            SystemCommands.RestoreWindow(window);
        }
    }

    [RelayCommand]
    private void Close(Window window)
    {
        SystemCommands.CloseWindow(window);
    }
}
