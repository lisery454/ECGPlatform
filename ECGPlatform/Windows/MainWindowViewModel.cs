namespace ECGPlatform;

internal partial class MainWindowViewModel : ObservableObject
{
    #region Public Property

    [ObservableProperty] private PagesName _currentPageName;

    #endregion

    #region Private Field

    private readonly ILogger _logger;

    #endregion

    #region Constructor

    public MainWindowViewModel(ILogger logger)
    {
        _logger = logger;
        _currentPageName = PagesName.LocalDataPage;
        _logger.Information($"{typeof(MainWindowViewModel)} Create.");
    }

    #endregion

    #region Commands

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

    #endregion
}