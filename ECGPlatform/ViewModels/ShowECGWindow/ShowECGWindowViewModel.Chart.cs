using LiveChartsCore.Measure;

namespace ECGPlatform;

public partial class ShowECGWindowViewModel
{
    [ObservableProperty] private ObservableCollection<Axis> _xAxes;
    [ObservableProperty] private ObservableCollection<Axis> _yAxes;
    [ObservableProperty] private DrawMarginFrame _drawMarginFrame;
    [ObservableProperty] private ObservableCollection<ISeries> _series;
    [ObservableProperty] private float _yGridValue = 0.5f;
    [ObservableProperty] private float _xGridValue = 200;

    [ObservableProperty] private float _width;
    [ObservableProperty] private float _height;

    private Lazy<ObjectPool<TextBlock>> _waveLabelTextPool;
    private Canvas WaveLabelCanvas => ((ShowECGWindow)BindingWindow!).WaveLabelCanvas;
    private Canvas LabelTextCanvas => ((ShowECGWindow)BindingWindow!).LabelTextCanvas;
    private Canvas IntervalTextCanvas => ((ShowECGWindow)BindingWindow!).IntervalTextCanvas;

    public const float YLimit = 2;
    public const float GridWidth = 40;
    public const float DistanceBetweenSeries = 1.8f;

    private Lazy<ObjectPool<CharLabel>> _charLabelsPool;
    private Lazy<ObjectPool<TimeLabel>> _timeLabelsPool;

    private static SKColor LabelColor => GetSKColor("ColorPrimaryAlpha90");
    private static SKColor SeparatorColor => GetSKColor("ColorOppositeAlpha40");
    private static SKColor SubseparatorsColor => GetSKColor("ColorOppositeAlpha20");
    private static SKColor TickColor => GetSKColor("ColorOppositeAlpha40");
    private static SKColor LineColor => GetSKColor("ColorPrimaryAlpha90");
    private static string FontFamily => "Chill Round Gothic Regular";

    private void UpdateYAxes(int count)
    {
        YAxes = new ObservableCollection<Axis> { BuildYAxis(count) };
    }

    private void UpdateChartSize(long timeInterval, int waveCount)
    {
        Width = timeInterval / XGridValue * GridWidth;
        Height = ((waveCount - 1) * DistanceBetweenSeries + 2 * YLimit) / YGridValue * GridWidth;
    }

    private void UpdateLineSeries(List<List<PointData>> points)
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

    private void UpdateWaveLabel()
    {
        foreach (TextBlock child in WaveLabelCanvas.Children)
        {
            if (child.Visibility == Visibility.Visible)
                _waveLabelTextPool.Value.Release(child);
        }

        for (var i = 0; i < WaveDataCollection.Count; i++)
        {
            var pointD = new LvcPointD(100, GetChartCoordY(WaveDataCollection[i][0].value, i));
            if (!IsYInProperInterval(pointD.Y)) continue;
            if (!IsXInProperInterval(pointD.X)) continue;

            var lvcPointD = CartesianChart.ScaleDataToPixels(pointD);


            var text = i switch
            {
                0 => "I",
                1 => "II",
                2 => "III",
                _ => "i"
            };
            _waveLabelTextPool.Value.Get(block =>
            {
                block.Visibility = Visibility.Visible;
                block.Text = text;
                Canvas.SetLeft(block, lvcPointD.X);
                Canvas.SetTop(block, lvcPointD.Y);
            });
        }
    }

    private void UpdateLabelTextAndIntervalText()
    {
        // label text
        {
            var chart = CartesianChart;
            var canvas = LabelTextCanvas;
            var pool = _charLabelsPool.Value;

            foreach (CharLabel child in canvas.Children)
                pool.Release(child, charLabel => charLabel.Visibility = Visibility.Hidden);

            foreach (var p in RPeakPoints)
            {
                var lvcPointD0 = new LvcPointD(p.Time, p.Values[0]);
                var point = chart.ScaleDataToPixels(lvcPointD0);
                pool.Get(charLabel =>
                {
                    charLabel.Visibility = Visibility.Visible;
                    charLabel.Char = p.Letter;
                    Canvas.SetLeft(charLabel, point.X);
                });
            }
        }

        // interval text
        {
            var chart = CartesianChart;
            var canvas = IntervalTextCanvas;
            var pool = _timeLabelsPool.Value;
        
            foreach (TimeLabel child in canvas.Children)
                pool.Release(child, timeLabel => timeLabel.Visibility = Visibility.Hidden);
        
            long? lastTime = null;
            double? lastPos = null;
            foreach (var p in RPeakPoints)
            {
                var time = p.Time;
        
                var lvcPointD0 = new LvcPointD(time, 0);
                var point = chart.ScaleDataToPixels(lvcPointD0);
                var pos = point.X;
        
                if (lastTime.HasValue && lastPos.HasValue)
                {
                    var l = lastTime.Value;
                    var d = lastPos.Value;
                    pool.Get(timeLabel =>
                    {
                        timeLabel.Visibility = Visibility.Visible;
                        timeLabel.TimeInterval = time - l;
                        Canvas.SetLeft(timeLabel, (pos + d) / 2d);
                    });
                }
        
                lastTime = time;
                lastPos = pos;
            }
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
        return new SKColor(color.R, color.G, color.B, color.A);
    }

    private bool IsYInProperInterval(double y)
    {
        return y >= -2 * YLimit - (_ecgFileManager!.WaveCount - 1) * DistanceBetweenSeries * 2 && y <= 0;
    }

    private bool IsXInProperInterval(double x)
    {
        return x >= CurrentTime && x < CurrentTime + TimeInterval && x >= 0 && x < AllMilliSeconds;
    }


    private double GetChartCoordY(double val, int i)
    {
        return val - DistanceBetweenSeries * i - YLimit;
    }

    #endregion
}