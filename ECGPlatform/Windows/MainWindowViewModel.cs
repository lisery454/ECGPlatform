using System.Windows;

namespace ECGPlatform;

partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _message;

    private readonly ILogger _logger;

    public MainWindowViewModel(ILogger logger)
    {
        _logger = logger;
        _message = "Hello, World";
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
