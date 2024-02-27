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
    [ObservableProperty] private string _textBoxInputCurrentTimeStr;
    [ObservableProperty] private List<HighlightPointData> _highlightPoints = new();
    [ObservableProperty] private Func<double, bool> _isYInProperInterval;
    [ObservableProperty] private Func<float, int, double> _getChartCoordY;

    private const long TimePerMouseWheel = 600;

    private readonly Animator _currentTimeAnimator;


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
        _xAxes = new ObservableCollection<Axis> { BuildXAxis() };
        _yAxes = new ObservableCollection<Axis> { BuildYAxis() };
        _drawMarginFrame = BuildDrawMarginFrame();
        _series = new ObservableCollection<ISeries>();
        _textBoxInputCurrentTimeStr = TimeFormatter.MircoSecondsToString(_currentTime);
        _currentTimeAnimator = new Animator(
            () => CurrentTime,
            value => CurrentTime = (long)value,
            TimeSpan.FromSeconds(0.016f),
            new EaseOutSquare(1, 4, TimeSpan.FromSeconds(0.1f))
        );

        _isYInProperInterval = y => y >= -2 * YLimit -
            (_ecgFileManager!.WaveCount - 1) * DistanceBetweenSeries * 2 && y <= 0;
        _getChartCoordY = (val, i) => val - DistanceBetweenSeries * i - YLimit;
    }
}

// property changed
public partial class ShowECGWindowViewModel
{
    partial void OnWaveDataCollectionChanged(List<List<PointData>> value)
    {
        UpdateChartSize(TimeInterval, WaveDataCollection.Count);
        UpdateYAxes(value.Count);
        UpdateLineSeries(value);
    }

    partial void OnTimeIntervalChanged(long value)
    {
        _ = value;
        UpdateChartSize(TimeInterval, WaveDataCollection.Count);
        UpdateWaveData(CtsUtils.Refresh(ref _updateWaveDataCts).Token).AwaitThrow();
    }

    partial void OnCurrentTimeChanged(long value)
    {
        _ = value;
        UpdateChartSize(TimeInterval, WaveDataCollection.Count);
        TextBoxInputCurrentTimeStr = TimeFormatter.MircoSecondsToString(CurrentTime);
        UpdateWaveData(CtsUtils.Refresh(ref _updateWaveDataCts).Token).AwaitThrow();
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
        // await UpdateRPeaksData(CtsUtils.Refresh(ref _updateRPeaksDataCts).Token);

        IsLoadingData = false;
    }

    [RelayCommand]
    private void ChartBorderSizeChanged(Border border)
    {
        var newTimeInterval = (long)(border.ActualWidth / GridWidth * XGridValue);
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
            _currentTimeAnimator.NoAnimateChangeTarget(milliSeconds);
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
            _currentTimeAnimator.NoAnimateChangeTarget(milliSeconds);
    }
}

// utils
public partial class ShowECGWindowViewModel
{
}