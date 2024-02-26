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
    [ObservableProperty] private string _textBoxInputCurrentTimeStr;
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

        _currentTime = 0;
        _chartViewModel = new ChartViewModel();
        _textBoxInputCurrentTimeStr = TimeFormatter.MircoSecondsToString(_currentTime);
        _currentTimeAnimator = new Animator<long>(() => CurrentTime, TimeSpan.FromSeconds(0.034f),
            (current, target, _) =>
            {
                if (Math.Abs(current - target) < 1)
                    return;

                if (Math.Abs(current - target) < 5)
                {
                    CurrentTime = target;
                }
                else
                {
                    CurrentTime = (long)(current + (target - current) * 0.6f);
                }
            });
    }
}

// property changed
public partial class ShowECGWindowViewModel
{
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
        TextBoxInputCurrentTimeStr = TimeFormatter.MircoSecondsToString(CurrentTime);
        UpdateWaveData(CtsUtils.Refresh(ref _updateWaveDataCts).Token).Await();
    }
}

// Commands
public partial class ShowECGWindowViewModel
{
    [RelayCommand]
    private async Task LoadData()
    {
        _ecgFileManager?.Dispose();
        _ecgFileManager = new ECGFileManager(EcgIndex!);

        AllMilliSeconds = _ecgFileManager.TotalTime;
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
        e.Handled = true;
    }

    [RelayCommand]
    private void CurrentTimeTextBox_OnLostKeyboardFocus()
    {
        if (!TimeFormatter.TryParseTimeMsFromStr(TextBoxInputCurrentTimeStr, out var milliSeconds))
        {
            TextBoxInputCurrentTimeStr = TimeFormatter.MircoSecondsToString(CurrentTime);
            return;
        }

        if (milliSeconds >= 0 && milliSeconds <= AllMilliSeconds - TimeInterval)
            _currentTimeAnimator.ChangeTarget(milliSeconds);
    }

    [RelayCommand]
    private void CurrentTimeTextBox_OnTextInput(TextCompositionEventArgs e)
    {
        if (e.Text == "\r")
        {
            Keyboard.ClearFocus();
        }
    }

    [RelayCommand]
    private void SliderValueChanged(RoutedPropertyChangedEventArgs<double> e)
    {
        var milliSeconds = (long)e.NewValue;
        if (milliSeconds >= 0 && milliSeconds <= AllMilliSeconds - TimeInterval)
            _currentTimeAnimator.ChangeTarget(milliSeconds);
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
        foreach (var i in Enumerable.Range(0, _ecgFileManager.WaveCount))
        {
            try
            {
                var data = await _ecgFileManager.GetRangedWaveDataAsync(i, CurrentTime, TimeInterval, 10,
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
        // _logger.Information("Wave Data Load Success.");
    }
}