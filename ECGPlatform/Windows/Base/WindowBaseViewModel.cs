namespace ECGPlatform;

public partial class WindowBaseViewModel : ObservableObject
{
    private WindowBase? _bindingWindow;

    public WindowBase? BindingWindow
    {
        get => _bindingWindow;
        set
        {
            if (_bindingWindow != null) _bindingWindow.Closed -= OnClosed;
            _bindingWindow = value;
            if (_bindingWindow != null) _bindingWindow.Closed += OnClosed;
        }
    }

    public event Action? Closed;

    private void OnClosed(object? sender, EventArgs e)
    {
        Closed?.Invoke();
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