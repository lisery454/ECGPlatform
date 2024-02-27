namespace ECGPlatform;

public partial class HighlightPointGroup
{
    public HighlightPointGroup()
    {
        InitializeComponent();

        _highLightPointPool = new ObjectPool<HighlightPoint>(
            () =>
            {
                var highlightPoint = new HighlightPoint
                {
                    Visibility = Visibility.Collapsed
                };
                HighlightPointCanvas.Children.Add(highlightPoint);
                return highlightPoint;
            }, 20);
    }

    private readonly ObjectPool<HighlightPoint> _highLightPointPool;
}

public partial class HighlightPointGroup
{
    #region HighlightPoints

    public static readonly DependencyProperty HighlightPointsProperty =
        DependencyProperty.Register(nameof(HighlightPoints), typeof(List<HighlightPointData>),
            typeof(HighlightPointGroup),
            new FrameworkPropertyMetadata(new List<HighlightPointData>(), OnHighlightPointDataChanged)
                { BindsTwoWayByDefault = true });

    public List<HighlightPointData> HighlightPoints
    {
        get => (List<HighlightPointData>)GetValue(HighlightPointsProperty);
        set => SetValue(HighlightPointsProperty, value);
    }

    private static void OnHighlightPointDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (d as HighlightPointGroup)!;
        foreach (HighlightPoint child in self.HighlightPointCanvas.Children)
            self._highLightPointPool.Release(child, point => point.Visibility = Visibility.Hidden);

        foreach (var highlightPointData in self.HighlightPoints)
        {
            var time = highlightPointData.Time;
            for (var i = 0; i < highlightPointData.Value.Count; i++)
            {
                var val = highlightPointData.Value[i];

                var lvcPointD = new LvcPointD(time,
                    self.GetChartCoordY(val, i)); 

                if (!self.IsYInProperInterval(lvcPointD.Y)) continue;

                var point = self.CartesianChart.ScaleDataToPixels(lvcPointD);

                self._highLightPointPool.Get(highlightPoint =>
                {
                    highlightPoint.Visibility = Visibility.Visible;
                    // highlightPoint.HighlightPoint = p;
                    // highlightPoint.PointLeftClickedCommand = showECGChart.PointLeftClickedCommand;
                    // highlightPoint.IsSelected = rPeakPoint.HighlightPoint == showECGChart.HighlightPoint;
                    Canvas.SetLeft(highlightPoint, point.X);
                    Canvas.SetTop(highlightPoint, point.Y);
                });
            }
        }
    }

    #endregion


    #region CartesianChart

    public static readonly DependencyProperty CartesianChartProperty =
        DependencyProperty.Register(nameof(CartesianChart), typeof(CartesianChart),
            typeof(HighlightPointGroup),
            new FrameworkPropertyMetadata(null)
                { BindsTwoWayByDefault = true });

    public CartesianChart CartesianChart
    {
        get => (CartesianChart)GetValue(CartesianChartProperty);
        set => SetValue(CartesianChartProperty, value);
    }

    #endregion

    #region IsYInProperInterval

    public static readonly DependencyProperty IsYInProperIntervalProperty =
        DependencyProperty.Register(nameof(IsYInProperInterval), typeof(Func<double, bool>),
            typeof(HighlightPointGroup),
            new FrameworkPropertyMetadata(null)
                { BindsTwoWayByDefault = false });

    public Func<double, bool> IsYInProperInterval
    {
        get => (Func<double, bool>)GetValue(IsYInProperIntervalProperty);
        set => SetValue(IsYInProperIntervalProperty, value);
    }

    #endregion

    #region GetChartCoordY

    public static readonly DependencyProperty GetChartCoordYProperty =
        DependencyProperty.Register(nameof(GetChartCoordY), typeof(Func<float, int, double>),
            typeof(HighlightPointGroup),
            new FrameworkPropertyMetadata(null)
                { BindsTwoWayByDefault = false });

    public Func<float, int, double> GetChartCoordY
    {
        get => (Func<float, int, double>)GetValue(GetChartCoordYProperty);
        set => SetValue(GetChartCoordYProperty, value);
    }

    #endregion
}