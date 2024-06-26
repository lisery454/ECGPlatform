﻿namespace ECGPlatform;

public partial class ShowECGWindowViewModel
{
    partial void OnWaveDataCollectionChanged(List<List<PointData>> value)
    {
        UpdateMainChartUI();
    }

    partial void OnShowECGWaveModeChanged(ShowECGWaveMode value)
    {
        UpdateMainChartUI();
    }

    partial void OnXGridValueEnumChanged(XGridValue value)
    {
        XGridValue = value switch
        {
            ECGPlatform.XGridValue._400_ => 400,
            ECGPlatform.XGridValue._200_ => 200,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    partial void OnXGridValueChanged(float value)
    {
        ChartBorderSizeChanged(ChartBorder);
        UpdateMainChartUI();
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

    partial void OnSearchPartRPointDataChanged(ObservableCollection<HighlightPointData> value)
    {
        TotalSearchLabelCount = value.Count;
    }

    partial void OnSearchRPeakLabelChanged(RPeakLabel value)
    {
        UpdateSearchRPointData(CtsUtils.Refresh(ref _updateSearchRPointDataCts).Token).AwaitThrow();
    }

    partial void OnMarkIntervalPointsData0Changed(HighlightPointData? value)
    {
        UpdateMarkIntervalPointDisplay();

        if (MarkIntervalPointsData0 != null && MarkIntervalPointsData1 != null) IsMarkable = true;
        else IsMarkable = false;
    }
    
    partial void OnMarkIntervalPointsData1Changed(HighlightPointData? value)
    {
        UpdateMarkIntervalPointDisplay();

        if (MarkIntervalPointsData0 != null && MarkIntervalPointsData1 != null) IsMarkable = true;
        else IsMarkable = false;
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
        if (ShowECGWaveMode == ShowECGWaveMode.ALL)
        {
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
        }
        else
        {
            var j = ShowECGWaveMode switch
            {
                ShowECGWaveMode.I => 0,
                ShowECGWaveMode.II => 1,
                ShowECGWaveMode.III => 2,
                _ => throw new ArgumentOutOfRangeException()
            };
            foreach (var pointData in RPeakPoints)
            {
                var time = pointData.Time;
                var val = pointData.Values[j];

                var lvcPointD = new LvcPointD(time, GetChartCoordY(val, 0));

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

        // 更新顶部的字符和时间间隔
        UpdateLabelTextAndIntervalText();
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