namespace ECGPlatform;

public partial class LocalDataPageViewModel : ObservableObject
{
    public ObservableCollection<LocalECGDataItem> LocalData { get; set; }

    [ObservableProperty] private LocalECGDataItem? _selectedItem;

    private readonly ILogger _logger;

    public LocalDataPageViewModel(ILogger logger)
    {
        _logger = logger;
        LocalData = new ObservableCollection<LocalECGDataItem>();
        _selectedItem = null;
    }

    [RelayCommand]
    private async Task InitLocalData()
    {
        // var localDataDirectoryPath = App.Container.Get<ISettingManager>().CurrentSetting.LocalDataDirectoryPath;
        var localDataDirectoryPath = @"E:\Data\毕业设计\data";
        var directory = new DirectoryInfo(localDataDirectoryPath);
        if (!directory.Exists) return;
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
                }
                catch (Exception e)
                {
                    _logger.Warning("读取index.yaml文件错误：" + e.Message);
                }
            }
            else _logger.Warning($"在{subDirectory.FullName}中找不到index.yaml文件");
        }
    }
}