namespace ECGPlatform;

public partial class LocalDataPageViewModel : ObservableObject
{
    public ObservableCollection<LocalECGDataItem> LocalData { get; set; }

    private readonly ILogger _logger;
    private readonly ISettingManager _settingManager;
    private readonly IndexFileService _indexFileService;

    public LocalDataPageViewModel(ILogger logger, ISettingManager settingManager,
        IndexFileService indexFileService)
    {
        _logger = logger;
        _settingManager = settingManager;
        _indexFileService = indexFileService;
        LocalData = new ObservableCollection<LocalECGDataItem>();
        logger.Information("Local Data Page ViewModel Create.");
    }

    [RelayCommand]
    private async Task InitLocalData()
    {
        var localDataDirectoryPath = _settingManager.CurrentSetting.LocalDataDirectoryPath;
        var directory = new DirectoryInfo(localDataDirectoryPath);
        if (!directory.Exists)
        {
            _logger.Warning("Local data directory not exists, load local data fail.");
            return;
        }

        foreach (var subDirectory in directory.GetDirectories())
        {
            var fileInfos = subDirectory.GetFiles("index.yaml");
            if (fileInfos.Length == 1)
            {
                try
                {
                    var indexFilePath = fileInfos[0].FullName;
                    var ecgIndex = await _indexFileService.Read(indexFilePath);
                    LocalData.Add(new LocalECGDataItem(ecgIndex, indexFilePath, ecgIndex.Title));
                    _logger.Information($"Load local file {indexFilePath} success.");
                }
                catch (Exception e)
                {
                    _logger.Warning("read index.yaml file fail：" + e.Message);
                }
            }
            else _logger.Warning($"In {subDirectory.FullName}, can't find index.yaml file");
        }
    }


    [RelayCommand]
    private void OpenLocalData(object? selectedItem)
    {
        if (selectedItem == null) return;
        var localECGDataItem = (LocalECGDataItem)selectedItem;
        var mainWindow = App.Current.Services.GetService<MainWindow>()!;
        var showECGWindow = App.Current.Services.GetService<ShowECGWindow>()!;
        // showECGWindow.Owner = mainWindow;
        mainWindow.Hide();
        var showECGWindowViewModel = (ShowECGWindowViewModel)showECGWindow.DataContext;
        showECGWindowViewModel.EcgIndex = localECGDataItem.ECGIndex;
        showECGWindowViewModel.Closed += () => { mainWindow.Show(); };
        showECGWindow.Show();
    }
}