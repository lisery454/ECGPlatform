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

    partial void OnWaveDataCollectionChanged(List<List<PointData>> value)
    {
        YAxes = new ObservableCollection<Axis> { BuildYAxis(value.Count) };
        Series.Clear();
        foreach (var list in value)
        {
            Series.Add(BuildLineSeries(list.Select(data => new ObservablePoint(data.time, data.value)).ToList()));
        }
    }

    public ShowECGWindowViewModel(ILogger logger)
    {
        _isLoadingData = true;
        _logger = logger;
        Closed += () => _ecgFileManager?.Dispose();

        _xAxes = new ObservableCollection<Axis> { BuildXAxis() };
        _yAxes = new ObservableCollection<Axis> { BuildYAxis() };
        _drawMarginFrame = BuildDrawMarginFrame();
        _series = new ObservableCollection<ISeries>();
    }

    [RelayCommand]
    private async Task LoadData()
    {
        _ecgFileManager?.Dispose();
        _ecgFileManager = new ECGFileManager(EcgIndex!);

        AllMilliSeconds = _ecgFileManager.waveDataReaders.First().TotalTime;
        CurrentTime = 0;
        TimeInterval = 5000;

        await UpdateWaveData();

        IsLoadingData = false;
    }
}

// update functions
public partial class ShowECGWindowViewModel
{
    private CancellationTokenSource? _updateWaveDataCts;

    private async Task UpdateWaveData()
    {
        if (_ecgFileManager == null) return;

        if (_updateWaveDataCts != null)
        {
            _updateWaveDataCts?.Cancel();
            _updateWaveDataCts?.Dispose();
        }

        _updateWaveDataCts = new CancellationTokenSource();

        var result = new List<List<PointData>>();
        foreach (var waveDataReader in _ecgFileManager.waveDataReaders)
        {
            try
            {
                var data = await waveDataReader.GetDataParallelAsync(CurrentTime, TimeInterval, 10,
                    _updateWaveDataCts.Token);
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

        WaveDataCollection = result;
        _updateWaveDataCts?.Dispose();
        _updateWaveDataCts = null;
        _logger.Information("Wave Data Load Success.");
    }
}

// charts
public partial class ShowECGWindowViewModel
{
    [ObservableProperty] private ObservableCollection<Axis> _xAxes;
    [ObservableProperty] private ObservableCollection<Axis> _yAxes;
    [ObservableProperty] private DrawMarginFrame _drawMarginFrame;
    [ObservableProperty] private ObservableCollection<ISeries> _series;
    [ObservableProperty] private float _yGridWidth = 0.5f;
    [ObservableProperty] private float _xGridWidth = 200;

    private const float YLimit = 2;

    // private float _gridWidth;
    private const float DistanceBetweenSeries = 1.5f;

    private static SKColor LabelColor => GetSKColor("ColorPrimary");
    private static SKColor SeparatorColor => GetSKColor("ColorPrimary");
    private static SKColor SubseparatorsColor => GetSKColor("ColorPrimary");
    private static SKColor TickColor => GetSKColor("ColorPrimary");
    private static SKColor LineColor => GetSKColor("ColorPrimary");
    private static string FontFamily => "Chill Round Gothic Regular";

    [RelayCommand]
    private void ChartUpdated()
    {
    }

    private ISeries BuildLineSeries(List<ObservablePoint> points)
    {
        return new LineSeries<ObservablePoint>
        {
            DataPadding = new LvcPoint(0f, 0f),
            GeometryStroke = null,
            GeometryFill = null,
            Values = points,
            Fill = null,
            LineSmoothness = 1,
            Stroke = new SolidColorPaint(LineColor, 1.5f),
        };
    }

    private Axis BuildXAxis()
    {
        var labelsPaint = new SolidColorPaint
        {
            Color = LabelColor,
            FontFamily = FontFamily,
            SKFontStyle = new SKFontStyle(SKFontStyleWeight.Bold, SKFontStyleWidth.Normal,
                SKFontStyleSlant.Italic)
        };

        var separatorsPaint = new SolidColorPaint
        {
            StrokeThickness = 1f,
            Color = SeparatorColor,
        };
        var labeler = (double d) =>
        {
            var ms = (int)d;
            var s = ms / 1000;
            ms %= 1000;
            var m = s / 60;
            s %= 60;
            var h = m / 60;
            m %= 60;
            return ms % 1000 == 0 ? $"{h}:{m:D2}:{s:D2}" : string.Empty;
        };
        var subseparatorsPaint = true
            ? new SolidColorPaint
            {
                Color = SubseparatorsColor,
                StrokeThickness = 0.3f,
                // PathEffect = new DashEffect(new float[] { 3, 3 })
            }
            : new SolidColorPaint
            {
                Color = SKColors.Transparent,
            };
        var ticksPaint = new SolidColorPaint
        {
            StrokeThickness = 1f,
            Color = TickColor,
        };


        return new Axis
        {
            SeparatorsPaint = separatorsPaint,
            MinStep = XGridWidth,
            ForceStepToMin = true,
            Labeler = labeler,
            LabelsPaint = labelsPaint,
            SubseparatorsPaint = subseparatorsPaint,
            SubseparatorsCount = 4,
            TicksPaint = ticksPaint,
        };
    }

    private Axis BuildYAxis(int waveCount = 3)
    {
        var labelsPaint = new SolidColorPaint
        {
            Color = LabelColor,
            FontFamily = FontFamily,
            SKFontStyle = new SKFontStyle(SKFontStyleWeight.Bold, SKFontStyleWidth.Normal,
                SKFontStyleSlant.Oblique)
        };
        string Labeler(double d) => string.Empty;
        var separatorsPaint = new SolidColorPaint
        {
            Color = SeparatorColor,
            StrokeThickness = 1f,
        };
        var subseparatorsPaint = true
            ? new SolidColorPaint
            {
                Color = SubseparatorsColor,
                StrokeThickness = 0.3f,
            }
            : new SolidColorPaint
            {
                Color = SKColors.Transparent,
            };

        return new Axis
        {
            LabelsPaint = labelsPaint,
            LabelsAlignment = Align.End,
            Labeler = Labeler,
            TextSize = 10,
            MinStep = YGridWidth,
            ForceStepToMin = true,
            MaxLimit = YLimit + waveCount * DistanceBetweenSeries,
            MinLimit = -YLimit,
            UnitWidth = 1,
            SeparatorsPaint = separatorsPaint,
            SubseparatorsPaint = subseparatorsPaint,
            SubseparatorsCount = 4
        };
    }

    private DrawMarginFrame BuildDrawMarginFrame()
    {
        return new DrawMarginFrame
        {
            Fill = null,
            Stroke = null,
        };
    }

    private static SKColor GetSKColor(string name)
    {
        var color = (Color)Application.Current.FindResource(name)!;
        return new SKColor(color.R, color.G, color.B);
    }
}