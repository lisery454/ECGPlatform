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
    }
}