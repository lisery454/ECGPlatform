namespace ECGPlatform;

public partial class RemotePageViewModel : ObservableObject
{
    private readonly IHttpManager _httpManager;
    public ObservableCollection<LabelTask> TaskData { get; set; } = new();
    public ObservableCollection<LocalECGDataItem> LocalData { get; set; } = new();
    private readonly ProgramConstants _programConstants;
    private readonly IndexFileService _indexFileService;
    private readonly ILogger _logger;


    public RemotePageViewModel(IHttpManager httpManager, ILogger logger, ProgramConstants programConstants,
        IndexFileService indexFileService)
    {
        _httpManager = httpManager;
        _logger = logger;
        _programConstants = programConstants;
        _indexFileService = indexFileService;
    }

    [RelayCommand]
    private async Task GetTaskListWindow()
    {
        var labelTasks = await _httpManager.TaskList();

        foreach (var labelTask in labelTasks)
        {
            TaskData.Add(labelTask);
        }
    }

    [RelayCommand]
    private async Task ShowFragment(object? selectedItem)
    {
        if (selectedItem == null) return;
        var labelTask = (LabelTask)selectedItem;

        if (!Directory.Exists(Path.Combine(_programConstants.DefaultRemoteDataDirectoryPath, $"{labelTask.Id}")))
        {
            foreach (var fragmentId in labelTask.FragmentIds)
            {
                await _httpManager.GetFragment(labelTask.Id, fragmentId);
            }
        }

        await LoadLocalData(Path.Combine(_programConstants.DefaultRemoteDataDirectoryPath, $"{labelTask.Id}"));
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

    private async Task LoadLocalData(string path)
    {
        LocalData.Clear();
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        var directory = new DirectoryInfo(path);
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
}