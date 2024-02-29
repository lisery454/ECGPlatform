namespace ECGPlatform;

internal partial class MainWindowViewModel : WindowBaseViewModel
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
}