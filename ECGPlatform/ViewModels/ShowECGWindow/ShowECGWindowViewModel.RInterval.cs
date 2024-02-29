namespace ECGPlatform;

public partial class ShowECGWindowViewModel
{
    [ObservableProperty] private long _thumbCurrentTime;

    [ObservableProperty] private long _thumbTimeDuration;

    [ObservableProperty] private List<RIntervalData> _rIntervalsData = new();

    [ObservableProperty] private List<RIntervalData> _partRIntervalsData = new();


    [ObservableProperty] private List<Axis> _rIntervalXAxes = new();
    [ObservableProperty] private List<Axis> _partRIntervalXAxes = new();
    [ObservableProperty] private List<Axis> _rIntervalYAxes = new();
    [ObservableProperty] private List<Axis> _partRIntervalYAxes = new();
    [ObservableProperty] private DrawMarginFrame _rIntervalDrawMarginFrame;
    [ObservableProperty] private DrawMarginFrame _partRIntervalDrawMarginFrame;
    [ObservableProperty] private ObservableCollection<ISeries> _rIntervalSeries = new();
    [ObservableProperty] private ObservableCollection<ISeries> _partRIntervalSeries = new();

    private bool _isDown;
    private bool _isPartDown;

    public RectangularSection[] Thumbs { get; set; }
    public RectangularSection[] PartThumbs { get; set; }

    private long _partTimeDuration;


    private void UpdateThumbPos()
    {
        Thumbs[0].Xi = ThumbCurrentTime;
        Thumbs[0].Xj = ThumbCurrentTime + ThumbTimeDuration;
    }

    private void UpdatePartThumbPos()
    {
        PartThumbs[0].Xi = CurrentTime;
        PartThumbs[0].Xj = CurrentTime + _partTimeDuration;
    }


    private static SKColor ThumbColor => GetSKColor("ColorPrimaryAlpha60");
    private static SKColor RIntervalPointColor => GetSKColor("ColorPrimary");
    private static SKColor RIntervalYLabelColor => GetSKColor("ColorPrimary");
    private static SKColor RIntervalMarginFrameColor => GetSKColor("ColorMain50OnOpposite");


    private RectangularSection BuildRectangularSection()
    {
        return new RectangularSection
        {
            Fill = new SolidColorPaint(ThumbColor)
        };
    }

    private ISeries BuildRIntervalLineSeries(IEnumerable<ObservablePoint> points)
    {
        return new LineSeries<ObservablePoint>
        {
            DataPadding = new LvcPoint(0f, 0f),
            GeometryStroke = new SolidColorPaint(RIntervalPointColor),
            GeometryFill = new SolidColorPaint(RIntervalPointColor),
            GeometrySize = 0.7f,
            Values = points,
            Fill = null,
            LineSmoothness = 0,
            // Stroke = new SolidColorPaint(MySKColor.ColorPrimary, 1f),
            Stroke = null,
        };
    }

    private Axis BuildRIntervalXAxis()
    {
        return new Axis
        {
            SeparatorsPaint = null,
            LabelsPaint = null,
            TextSize = 4
        };
    }

    private Axis BuildRIntervalYAxis()
    {
        var labelsPaint = new SolidColorPaint
        {
            Color = RIntervalYLabelColor,
            FontFamily = FontFamily,
            SKFontStyle = new SKFontStyle(SKFontStyleWeight.Bold, SKFontStyleWidth.Normal,
                SKFontStyleSlant.Oblique)
        };


        return new Axis
        {
            LabelsPaint = labelsPaint,
            SeparatorsPaint = null,
            LabelsAlignment = Align.End,
            TextSize = 10,
            MinStep = 1000,
            MaxLimit = 2000,
            MinLimit = 0,
            ForceStepToMin = true
        };
    }

    private DrawMarginFrame BuildRIntervalDrawMarginFrame()
    {
        return new DrawMarginFrame
        {
            Fill = null,
            Stroke = new SolidColorPaint
            {
                StrokeThickness = 1f,
                Color = RIntervalMarginFrameColor,
            },
        };
    }


    private void UpdateRIntervalSeries()
    {
        RIntervalSeries[0] = BuildRIntervalLineSeries(RIntervalsData
            .Select(point => new ObservablePoint(point.Time, point.Interval))
            .ToList());
    }

    private void UpdatePartRIntervalSeries()
    {
        PartRIntervalSeries[0] = BuildRIntervalLineSeries(PartRIntervalsData.Select(point =>
            new ObservablePoint(point.Time, point.Interval)).ToList());
    }
}