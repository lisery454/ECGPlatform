namespace ECGPlatform;

public partial class ShowECGWindowViewModel
{
    private readonly Lazy<ObjectPool<HighlightPoint>> _rPeakPointPool;
    private readonly Lazy<ObjectPool<HighlightPoint>> _simplePointPool;
    private Canvas RPeakPointCanvas => ((ShowECGWindow)BindingWindow!).RPeakPointCanvas;
    private Canvas SimplePointCanvas => ((ShowECGWindow)BindingWindow!).SimplePointCanvas;

    private CartesianChart CartesianChart => ((ShowECGWindow)BindingWindow!).CartesianChart;
    private Border ChartBorder => ((ShowECGWindow)BindingWindow!).ChartBorder;

    private void HideAllRPeakPoints()
    {
        foreach (HighlightPoint child in RPeakPointCanvas.Children)
            _rPeakPointPool.Value.Release(child, point =>
            {
                point.Visibility = Visibility.Hidden;
                point.ClearBinding(HighlightPoint.ClickedCommandProperty);
                // point.Reset();
                point.IsSelected = false;
            });
    }

    private void SetHighlightPointByPixelPosition(Point position)
    {
        var time = (long)CartesianChart.ScalePixelsToData(new LvcPointD(position.X, position.Y)).X;
        if (time < CurrentTime || time >= CurrentTime + TimeInterval || WaveDataCollection.Count <= 0) return;

        var values = new List<Point>();
        for (var i = 0; i < WaveDataCollection.Count; i++)
        {
            values.Add(FindMinDistancePoint(i, time));
        }

        CurrentHighlightPointData =
            new HighlightPointData((long)values[0].X, values.Select(point => (float)point.Y).ToList());
    }

    private Point FindMinDistancePoint(int id, long time)
    {
        var wavePoints = WaveDataCollection[id];
        var point = wavePoints[0];
        var minValue = MathF.Abs(point.time - time);
        foreach (var wavePoint in wavePoints)
        {
            var abs = MathF.Abs(wavePoint.time - time);
            if (abs < minValue)
            {
                minValue = abs;
                point = wavePoint;
            }
        }

        return new Point(point.time, point.value);
    }

    private void UnSelectRPeaksPoint()
    {
        _rPeakPointPool.Value.ForeachNotAvail(rPeakPoint => rPeakPoint.IsSelected = false);
    }

    private void SetSelectRPeaksPoint()
    {
        _rPeakPointPool.Value.ForeachNotAvail(rPeakPoint =>
        {
            rPeakPoint.IsSelected = rPeakPoint.HighlightPointData == CurrentHighlightPointData;
        });
    }

    private void UnSelectSimplePoint()
    {
        _simplePointPool.Value.ForeachNotAvail(point =>
        {
            point.Visibility = Visibility.Hidden;
            point.IsSelected = false;
        });
    }

    private void SetSelectSimplePoint()
    {
        foreach (HighlightPoint child in SimplePointCanvas.Children)
            _simplePointPool.Value.Release(child, point =>
            {
                point.Visibility = Visibility.Hidden;
                // point.IsSelected = false;
            });

        if (CurrentHighlightPointData == null) return;
        var time = CurrentHighlightPointData.Time;
        var values = CurrentHighlightPointData.Values;

        for (var i = 0; i < values.Count; i++)
        {
            var pointD = new LvcPointD(time, GetChartCoordY(values[i], i));
            if (!IsYInProperInterval(pointD.Y)) continue;
            if (!IsXInProperInterval(pointD.X)) continue;

            var lvcPointD = CartesianChart.ScaleDataToPixels(pointD);
            _simplePointPool.Value.Get(point =>
            {
                point.IsSelected = true;
                point.Visibility = Visibility.Visible;
                Canvas.SetLeft(point, lvcPointD.X);
                Canvas.SetTop(point, lvcPointD.Y);
            });
        }
    }

    private void UpdateHighlightPoint()
    {
        if (CurrentHighlightPointData == null)
        {
            UnSelectRPeaksPoint();
            UnSelectSimplePoint();
        }
        else if (CurrentHighlightPointData.PointType == PointType.R_PEAKS_POINT)
        {
            SetSelectRPeaksPoint();
            UnSelectSimplePoint();
        }
        else if (CurrentHighlightPointData.PointType == PointType.SIMPLE_POINT)
        {
            SetSelectSimplePoint();
            UnSelectRPeaksPoint();
        }
    }
}