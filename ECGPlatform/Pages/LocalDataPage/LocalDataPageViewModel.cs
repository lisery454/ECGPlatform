namespace ECGPlatform;

public partial class LocalDataPageViewModel : ObservableObject
{
    public ObservableCollection<LocalECGDataItem> LocalData { get; set; }

    [ObservableProperty] private LocalECGDataItem? _selectedItem;

    private readonly ILogger _logger;
    private readonly ISettingManager _settingManager;

    public LocalDataPageViewModel(ILogger logger, ISettingManager settingManager)
    {
        _logger = logger;
        _settingManager = settingManager;
        LocalData = new ObservableCollection<LocalECGDataItem>();
        _selectedItem = null;
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
                    var ecgIndexFile = await ECGFileManager.ReadIndexFile(indexFilePath);
                    LocalData.Add(new LocalECGDataItem { Title = ecgIndexFile.Title, IndexFilePath = indexFilePath });
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
}