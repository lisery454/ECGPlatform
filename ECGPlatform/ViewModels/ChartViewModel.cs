using LiveChartsCore.Measure;

namespace ECGPlatform;

public partial class ChartViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Axis> _xAxes;
    [ObservableProperty] private ObservableCollection<Axis> _yAxes;
    [ObservableProperty] private DrawMarginFrame _drawMarginFrame;
    [ObservableProperty] private ObservableCollection<ISeries> _series;
    [ObservableProperty] private float _yGridValue = 0.5f;
    [ObservableProperty] private float _xGridValue = 200;

    [ObservableProperty] private float _width;
    [ObservableProperty] private float _height;

    private const float YLimit = 2;
    public const float GridWidth = 40;
    private const float DistanceBetweenSeries = 1.5f;

    public ChartViewModel()
    {
        _xAxes = new ObservableCollection<Axis> { BuildXAxis() };
        _yAxes = new ObservableCollection<Axis> { BuildYAxis() };
        _drawMarginFrame = BuildDrawMarginFrame();
        _series = new ObservableCollection<ISeries>();
    }

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

    public void UpdateYAxes(int count)
    {
        YAxes = new ObservableCollection<Axis> { BuildYAxis(count) };
    }

    public void UpdateChartSize(long timeInterval, int waveCount)
    {
        Width = timeInterval / XGridValue * GridWidth;
        Height = ((waveCount - 1) * DistanceBetweenSeries + 2 * YLimit) / YGridValue * GridWidth;
    }

    public void UpdateLineSeries(List<List<PointData>> points)
    {
        Series.Clear();
        for (var i = 0; i < points.Count; i++)
        {
            var p = points[i];
            var index = i;
            Series.Add(BuildLineSeries(p.Select(data =>
                new ObservablePoint(data.time, data.value - YLimit - index * DistanceBetweenSeries))));
        }
    }


    #region Utils

    private ISeries BuildLineSeries(IEnumerable<ObservablePoint> points)
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
            MinStep = XGridValue,
            ForceStepToMin = true,
            Labeler = labeler,
            Position = AxisPosition.End,
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
            MinStep = YGridValue,
            ForceStepToMin = true,
            MaxLimit = 0,
            MinLimit = -2 * YLimit - (waveCount - 1) * DistanceBetweenSeries,
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

    #endregion
}