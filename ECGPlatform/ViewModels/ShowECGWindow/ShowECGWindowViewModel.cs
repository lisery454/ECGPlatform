namespace ECGPlatform;

public partial class ShowECGWindowViewModel : WindowBaseViewModel
{
    /// <summary>
    /// 当前的ECGIndex信息
    /// </summary>
    public ECGIndex? EcgIndex { get; set; }

    /// <summary>
    /// logger
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// ECG读取文件类
    /// </summary>
    private ECGFileManager? _ecgFileManager;

    /// <summary>
    /// 当前是否正在加载数据
    /// </summary>
    [ObservableProperty] private bool _isLoadingData;

    /// <summary>
    /// 波形的数据
    /// </summary>
    [ObservableProperty] private List<List<PointData>> _waveDataCollection = new();

    /// <summary>
    /// 当前显示的时间间隔
    /// </summary>
    [ObservableProperty] private long _timeInterval;

    /// <summary>
    /// 当前的时间点
    /// </summary>
    [ObservableProperty] private long _currentTime;

    /// <summary>
    /// 波形的所有时间
    /// </summary>
    [ObservableProperty] private long _allMilliSeconds;

    /// <summary>
    /// 输入的时间字符串
    /// </summary>
    [ObservableProperty] private string _textBoxInputCurrentTimeStr;

    /// <summary>
    /// 所有R点的信息
    /// </summary>
    [ObservableProperty] private List<HighlightPointData> _rPeakPoints = new();

    /// <summary>
    /// 当前选中的点的信息，可能是R点，可能是普通点
    /// </summary>
    [ObservableProperty] private HighlightPointData? _currentHighlightPointData;

    /// <summary>
    /// 鼠标滚动位移的时间
    /// </summary>
    private const long TimePerMouseWheel = 600;

    /// <summary>
    /// 当前时间的动画器
    /// </summary>
    private readonly Animator _currentTimeAnimator;


    /// <summary>
    /// 当前是否可以去点击选中点
    /// </summary>
    private bool _canMouseMoveToSelectPoint;


    /// <summary>
    /// 当前要创建的点的标签
    /// </summary>
    [ObservableProperty] private RPeakLabel _createRPeakLabel;

    /// <summary>
    /// 当前要修改标签的点的标签
    /// </summary>
    [ObservableProperty] private RPeakLabel _updateRPeakLabel;

    /// <summary>
    /// 当前要搜索标签的点的标签
    /// </summary>
    [ObservableProperty] private RPeakLabel _searchRPeakLabel;

    /// <summary>
    /// 当前的波形展示模式
    /// </summary>
    [ObservableProperty] private ShowECGWaveMode _showECGWaveMode;

    /// <summary>
    /// 搜索窗口的可见性
    /// </summary>
    [ObservableProperty] private Visibility _searchPartVisibility;

    /// <summary>
    /// 所有的所有标签数量
    /// </summary>
    [ObservableProperty] private int _totalSearchLabelCount;

    /// <summary>
    /// 当前标签的R点数据
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<HighlightPointData> _searchPartRPointData;


    /// <summary>
    /// 是否正在搜索R点
    /// </summary>
    [ObservableProperty] private bool _isSearching;


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
            new EaseOutExpo(1, 4, 0.5f)
            // new EaseOutSquare(1, 4, TimeSpan.FromSeconds(0.1f))
        );

        _searchPartRPointData = new ObservableCollection<HighlightPointData>();
        _searchPartVisibility = Visibility.Collapsed;

        _rPeakPointPool = new Lazy<ObjectPool<HighlightPoint>>(() => new ObjectPool<HighlightPoint>(
            () =>
            {
                var highlightPoint = new HighlightPoint
                {
                    Visibility = Visibility.Hidden
                };
                RPeakPointCanvas.Children.Add(highlightPoint);
                return highlightPoint;
            }, 20));

        _simplePointPool = new Lazy<ObjectPool<HighlightPoint>>(() => new ObjectPool<HighlightPoint>(
            () =>
            {
                var highlightPoint = new HighlightPoint
                {
                    Visibility = Visibility.Hidden
                };
                SimplePointCanvas.Children.Add(highlightPoint);
                return highlightPoint;
            }, 3));

        _waveLabelTextPool = new Lazy<ObjectPool<TextBlock>>(() => new ObjectPool<TextBlock>(() =>
        {
            var textBlock = new TextBlock()
            {
                FontSize = 20, Text = "Null", Visibility = Visibility.Hidden
            };

            WaveLabelCanvas.Children.Add(textBlock);
            return textBlock;
        }, 3));

        _charLabelsPool = new Lazy<ObjectPool<CharLabel>>(() => new ObjectPool<CharLabel>(() =>
        {
            var charLabel = new CharLabel
            {
                Visibility = Visibility.Collapsed,
                Width = 24,
                Height = 24,
                Char = "S",
            };

            LabelTextCanvas.Children.Add(charLabel);
            return charLabel;
        }, 10));

        _timeLabelsPool = new Lazy<ObjectPool<TimeLabel>>(() => new ObjectPool<TimeLabel>(() =>
        {
            var timeLabel = new TimeLabel
            {
                Visibility = Visibility.Collapsed,
                Width = 20,
                Height = 25,
            };

            IntervalTextCanvas.Children.Add(timeLabel);
            return timeLabel;
        }, 10));

        _createRPeakLabel = RPeakLabel.NONE;
        _updateRPeakLabel = RPeakLabel.NONE;

        RIntervalXAxes.Add(BuildRIntervalXAxis());
        PartRIntervalXAxes.Add(BuildRIntervalXAxis());
        RIntervalYAxes.Add(BuildRIntervalYAxis());
        PartRIntervalYAxes.Add(BuildRIntervalYAxis());
        RIntervalSeries.Add(BuildRIntervalLineSeries(new List<ObservablePoint>()));
        PartRIntervalSeries.Add(BuildRIntervalLineSeries(new List<ObservablePoint>()));
        RIntervalDrawMarginFrame = BuildRIntervalDrawMarginFrame();
        PartRIntervalDrawMarginFrame = BuildRIntervalDrawMarginFrame();
        Thumbs = new[] { BuildRectangularSection() };
        PartThumbs = new[] { BuildRectangularSection() };
        _partTimeDuration = 3000;
        ThumbCurrentTime = 0;

        UpdateThumbPos();
        UpdatePartThumbPos();
    }
}