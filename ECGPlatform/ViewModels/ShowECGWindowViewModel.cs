using SimpleUtils;

namespace ECGPlatform;

public partial class ShowECGWindowViewModel : WindowBaseViewModel
{
    public ECGIndex? EcgIndex { get; set; }
    private readonly ILogger _logger;
    private ECGFileManager? _ecgFileManager;
    [ObservableProperty] private bool _isLoadingData;
    [ObservableProperty] private List<List<PointData>> _waveDataCollection = new();
    [ObservableProperty] private long _timeInterval;
    [ObservableProperty] private long _currentTime;
    [ObservableProperty] private long _allMilliSeconds;

    [ObservableProperty] private ChartViewModel _chartViewModel;

    partial void OnWaveDataCollectionChanged(List<List<PointData>> value)
    {
        ChartViewModel.UpdateChartSize(TimeInterval, WaveDataCollection.Count);
        ChartViewModel.UpdateYAxes(value.Count);
        ChartViewModel.UpdateLineSeries(value);
    }

    partial void OnTimeIntervalChanged(long value)
    {
        ChartViewModel.UpdateChartSize(TimeInterval, WaveDataCollection.Count);
        UpdateWaveData(Refresh(ref _updateWaveDataCts).Token).Await();
    }

    public ShowECGWindowViewModel(ILogger logger)
    {
        _isLoadingData = true;
        _logger = logger;
        Closed += () => _ecgFileManager?.Dispose();


        _chartViewModel = new ChartViewModel();
    }

    [RelayCommand]
    private async Task LoadData()
    {
        _ecgFileManager?.Dispose();
        _ecgFileManager = new ECGFileManager(EcgIndex!);

        AllMilliSeconds = _ecgFileManager.waveDataReaders.First().TotalTime;
        CurrentTime = 0;
        TimeInterval = 5000;

        await UpdateWaveData(Refresh(ref _updateWaveDataCts).Token);

        IsLoadingData = false;
    }

    [RelayCommand]
    private void ChartBorderSizeChanged(Border border)
    {
        TimeInterval = (long)(border.ActualWidth / ChartViewModel.GridWidth * ChartViewModel.XGridValue);
    }
}

// update functions
public partial class ShowECGWindowViewModel
{
    private CancellationTokenSource? _updateWaveDataCts;

    private CancellationTokenSource Refresh(ref CancellationTokenSource? source)
    {
        if (source != null)
        {
            source.Cancel();
            source.Dispose();
            source = null;
        }

        source = new CancellationTokenSource();
        return source;
    }

    private async Task UpdateWaveData(CancellationToken cancellationToken)
    {
        if (_ecgFileManager == null) return;

        var result = new List<List<PointData>>();
        foreach (var waveDataReader in _ecgFileManager.waveDataReaders)
        {
            try
            {
                var data = await waveDataReader.GetDataParallelAsync(CurrentTime, TimeInterval, 10,
                    cancellationToken);
                result.Add(data);
            }
            catch (OperationCanceledException)
            {
                // 正常取消获取数据操作
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
            }
        }

        if (cancellationToken.IsCancellationRequested) return;

        WaveDataCollection = result;
        _logger.Information("Wave Data Load Success.");
    }
}