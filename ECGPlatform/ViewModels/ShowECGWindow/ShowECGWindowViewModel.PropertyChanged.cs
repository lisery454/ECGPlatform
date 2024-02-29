namespace ECGPlatform;

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
        UpdatePartThumbPos();
    }

    partial void OnCurrentHighlightPointDataChanged(HighlightPointData? value)
    {
        UpdateHighlightPoint();
    }

    partial void OnRPeakPointsChanged(List<HighlightPointData> value)
    {
        _ = value;
        var highLightPointPool = _rPeakPointPool.Value;
        // 先取消显示当前显示的点 
        HideAllRPeakPoints();

        // 显示当前要显示的点
        foreach (var pointData in RPeakPoints)
        {
            var time = pointData.Time;
            for (var i = 0; i < pointData.Values.Count; i++)
            {
                var val = pointData.Values[i];

                var lvcPointD = new LvcPointD(time, GetChartCoordY(val, i));

                if (!IsYInProperInterval(lvcPointD.Y)) continue;

                var point = CartesianChart.ScaleDataToPixels(lvcPointD);

                highLightPointPool.Get(highlightPoint =>
                {
                    highlightPoint.Visibility = Visibility.Visible;
                    highlightPoint.HighlightPointData = pointData;

                    highlightPoint.Binding(HighlightPoint.ClickedCommandProperty).To(this,
                        new PropertyPath(nameof(HighlightPointPointClickedCommand)));

                    highlightPoint.IsSelected = CurrentHighlightPointData != null &&
                                                highlightPoint.HighlightPointData.Time ==
                                                CurrentHighlightPointData.Time;

                    Canvas.SetLeft(highlightPoint, point.X);
                    Canvas.SetTop(highlightPoint, point.Y);
                });
            }
        }

        UpdateHighlightPoint();
    }

    partial void OnThumbCurrentTimeChanged(long value)
    {
        _ = value;
        UpdatePartRIntervalData(CtsUtils.Refresh(ref _updatePartRIntervalDataCts).Token).AwaitThrow();
        UpdateThumbPos();
    }

    partial void OnThumbTimeDurationChanged(long value)
    {
        _ = value;
        UpdatePartRIntervalData(CtsUtils.Refresh(ref _updatePartRIntervalDataCts).Token).AwaitThrow();
        UpdateThumbPos();
    }

    partial void OnRIntervalsDataChanged(List<RIntervalData> value)
    {
        _ = value;
        UpdateRIntervalSeries();
    }

    partial void OnPartRIntervalsDataChanged(List<RIntervalData> value)
    {
        _ = value;
        UpdatePartRIntervalSeries();
    }
}