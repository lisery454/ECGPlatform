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
    private const long TimePerMouseWheel = 600;

    private readonly Animator<long> _currentTimeAnimator;
    public ShowECGWindowViewModel(ILogger logger)
    {
        _isLoadingData = true;
        _logger = logger;
        Closed += () =>
        {
            _currentTimeAnimator?.Dispose();
            _ecgFileManager?.Dispose();
        };

        _chartViewModel = new ChartViewModel();
        _currentTimeAnimator = new Animator<long>(() => CurrentTime, TimeSpan.FromSeconds(0.1f),
            (current, target, isTargetChanged) =>
            {
                if (Math.Abs(current - target) < 40)
                {
                    CurrentTime = target;
                }
                else
                {
                    CurrentTime = (long)(current + (target - current) * 0.5f);
                }
            });
    }

    partial void OnWaveDataCollectionChanged(List<List<PointData>> value)
    {
        ChartViewModel.UpdateChartSize(TimeInterval, WaveDataCollection.Count);
        ChartViewModel.UpdateYAxes(value.Count);
        ChartViewModel.UpdateLineSeries(value);
    }

    partial void OnTimeIntervalChanged(long value)
    {
        _ = value;
        ChartViewModel.UpdateChartSize(TimeInterval, WaveDataCollection.Count);
        UpdateWaveData(CtsUtils.Refresh(ref _updateWaveDataCts).Token).Await();
    }

    partial void OnCurrentTimeChanged(long value)
    {
        _ = value;
        ChartViewModel.UpdateChartSize(TimeInterval, WaveDataCollection.Count);
        UpdateWaveData(CtsUtils.Refresh(ref _updateWaveDataCts).Token).Await();
    }


    [RelayCommand]
    private async Task LoadData()
    {
        _ecgFileManager?.Dispose();
        _ecgFileManager = new ECGFileManager(EcgIndex!);

        AllMilliSeconds = _ecgFileManager.waveDataReaders.First().TotalTime;
        CurrentTime = 0;
        TimeInterval = 5000;

        await UpdateWaveData(CtsUtils.Refresh(ref _updateWaveDataCts).Token);

        IsLoadingData = false;
    }

    [RelayCommand]
    private void ChartBorderSizeChanged(Border border)
    {
        var newTimeInterval = (long)(border.ActualWidth / ChartViewModel.GridWidth * ChartViewModel.XGridValue);
        if (TimeInterval + CurrentTime > AllMilliSeconds)
            _currentTimeAnimator.ChangeTarget(AllMilliSeconds - newTimeInterval);

        TimeInterval = newTimeInterval;
    }

    [RelayCommand]
    private void ChartMouseWheel(MouseWheelEventArgs e)
    {
        if (e.Handled) return;
        var newCurrentTime = CurrentTime + TimePerMouseWheel * MathF.Sign(e.Delta);
        _currentTimeAnimator.ChangeTarget(MathUtils.Clamp(newCurrentTime, 0, AllMilliSeconds - TimeInterval));
        // CurrentTime = MathUtils.Clamp(newCurrentTime, 0, AllMilliSeconds - TimeInterval);
        e.Handled = true;
    }
}

// update functions
public partial class ShowECGWindowViewModel
{
    private CancellationTokenSource? _updateWaveDataCts;

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