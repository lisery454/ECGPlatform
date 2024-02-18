namespace ECGPlatform;

public partial class WindowBaseViewModel : ObservableObject
{
    public WindowBase? BindingWindow { get; set; }

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