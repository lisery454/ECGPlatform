﻿namespace ECGPlatform;

public partial class ShowECGWindowViewModel
{
    [RelayCommand]
    private async Task LoadData()
    {
        ShowECGWaveMode = ShowECGWaveMode.ALL;

        // 加载文件
        _ecgFileManager?.Dispose();
        _ecgFileManager = new ECGFileManager(EcgIndex!);

        // 加载时间数据
        AllMilliSeconds = _ecgFileManager.TotalTime;
        CurrentTime = 0;
        TimeInterval = 5000;
        ThumbTimeDuration = _ecgFileManager.TotalTime / 20;

        // 加载波形数据
        await UpdateWaveData(CtsUtils.Refresh(ref _updateWaveDataCts).Token);

        // 加载R点间隔数据
        await UpdateRIntervalData(CtsUtils.Refresh(ref _updateRIntervalDataCts).Token);

        // 加载搜索R点的数据
        UpdateSearchRPointData(CtsUtils.Refresh(ref _updateSearchRPointDataCts).Token).AwaitThrow();

        IsLoadingData = false;
        UpdateWaveLabel();
    }

    [RelayCommand]
    private void SelectRPoint(HighlightPointData highlightPointData)
    {
        CurrentHighlightPointData = highlightPointData;

        // set current time
        if (CurrentHighlightPointData != null)
        {
            var time = CurrentHighlightPointData.Time;
            var begin = time - TimeInterval / 2;
            var end = time + TimeInterval / 2;
            long result;
            if (begin < 0) result = 0;
            else if (end >= AllMilliSeconds) result = AllMilliSeconds - TimeInterval;
            else result = begin;

            _currentTimeAnimator.NoAnimateChangeTarget(result);
        }
    }

    [RelayCommand]
    private void ShowSearchPart()
    {
        SearchPartVisibility = SearchPartVisibility switch
        {
            Visibility.Visible => Visibility.Collapsed,
            Visibility.Collapsed => Visibility.Visible,
            _ => Visibility.Visible
        };
    }

    [RelayCommand]
    private void ChartBorderSizeChanged(Border border)
    {
        // 修改时间间隔
        var newTimeInterval = (long)(border.ActualWidth / GridWidth * XGridValue);
        if (TimeInterval + CurrentTime > AllMilliSeconds)
            _currentTimeAnimator.NoAnimateChangeTarget(AllMilliSeconds - newTimeInterval);

        TimeInterval = newTimeInterval;
    }

    [RelayCommand]
    private void ChartMouseWheel(MouseWheelEventArgs e)
    {
        // if (e.Handled) return;
        // 修改当前时间
        // ar newCurrentTime = CurrentTime + TimePerMouseWheel * MathF.Sign(e.Delta);
        // _currentTimeAnimator.ChangeTarget(MathUtils.Clamp(newCurrentTime, 0, AllMilliSeconds - TimeInterval));
        // e.Handled = true;
    }

    [RelayCommand]
    private void CurrentTimeTextBox_OnLostKeyboardFocus()
    {
        // 输入是否合理
        if (!TimeFormatter.TryParseTimeMsFromStr(TextBoxInputCurrentTimeStr, out var milliSeconds))
        {
            TextBoxInputCurrentTimeStr = TimeFormatter.MircoSecondsToString(CurrentTime);
            return;
        }

        // 修改当前时间
        if (milliSeconds >= 0 && milliSeconds <= AllMilliSeconds - TimeInterval)
            _currentTimeAnimator.NoAnimateChangeTarget(milliSeconds);
    }

    [RelayCommand]
    private void CurrentTimeTextBox_OnTextInput(TextCompositionEventArgs e)
    {
        if (e.Text == "\r")
        {
            Keyboard.ClearFocus();
        }
    }

    [RelayCommand]
    private void SliderValueChanged(RoutedPropertyChangedEventArgs<double> e)
    {
        // 修改当前时间
        var milliSeconds = (long)e.NewValue;
        if (milliSeconds >= 0 && milliSeconds <= AllMilliSeconds - TimeInterval)
            _currentTimeAnimator.NoAnimateChangeTarget(milliSeconds);
    }

    [RelayCommand]
    private void HighlightPointPointClicked(HighlightPointData data)
    {
        // 设置当前的高亮点为这个点
        CurrentHighlightPointData = data;

        MarkIntervalPointsData0 = null;
        MarkIntervalPointsData1 = null;
        _isMark0 = false;
    }

    [RelayCommand]
    private async Task ChartUpdated()
    {
        // HideAllRPeakPoints();
        await UpdateRPeaksData(CtsUtils.Refresh(ref _updateRPeaksDataCts).Token);

        UpdateMarkIntervalPointDisplay();
        // UpdateLabelTextAndIntervalText();
    }

    [RelayCommand]
    private async Task UpdateRPoint()
    {
        var oldLabel = CurrentHighlightPointData!.Label!.Value;
        var newLabel = UpdateRPeakLabel;

        await _ecgFileManager!.UpdateRPeakPointLabel(CurrentHighlightPointData!.Time,
            oldLabel,
            UpdateRPeakLabel);

        var oldPointData = new HighlightPointData(CurrentHighlightPointData);

        CurrentHighlightPointData = new HighlightPointData(CurrentHighlightPointData!.Time,
            CurrentHighlightPointData!.Values, UpdateRPeakLabel);
        await ChartUpdated();

        if (oldLabel == SearchRPeakLabel)
        {
            var indexOf = SearchPartRPointData.IndexOf(oldPointData);
            SearchPartRPointData.RemoveAt(indexOf);
            TotalSearchLabelCount = SearchPartRPointData.Count;
        }

        if (newLabel == SearchRPeakLabel)
        {
            SearchPartRPointData.Add(new HighlightPointData(CurrentHighlightPointData!.Time,
                CurrentHighlightPointData!.Values, newLabel));
            TotalSearchLabelCount = SearchPartRPointData.Count;
        }
    }

    [RelayCommand]
    private async Task DeleteRPoint()
    {
        var oldLabel = CurrentHighlightPointData!.Label!.Value;

        await _ecgFileManager!.DeleteRPeak(CurrentHighlightPointData!.Time);
        var oldPointData = new HighlightPointData(CurrentHighlightPointData);

        CurrentHighlightPointData = null;
        await ChartUpdated();

        if (oldLabel == SearchRPeakLabel)
        {
            var indexOf = SearchPartRPointData.IndexOf(oldPointData);
            SearchPartRPointData.RemoveAt(indexOf);
            TotalSearchLabelCount = SearchPartRPointData.Count;
        }
    }

    [RelayCommand]
    private async Task CreateRPoint()
    {
        var newLabel = CreateRPeakLabel;

        await _ecgFileManager!.AddRPeakPointAsync(CurrentHighlightPointData!.Time, CreateRPeakLabel);
        CurrentHighlightPointData = new HighlightPointData(CurrentHighlightPointData!.Time,
            CurrentHighlightPointData!.Values, newLabel);

        await ChartUpdated();

        if (newLabel == SearchRPeakLabel)
        {
            SearchPartRPointData.Add(new HighlightPointData(CurrentHighlightPointData!.Time,
                CurrentHighlightPointData!.Values, newLabel));
            TotalSearchLabelCount = SearchPartRPointData.Count;
        }
    }

    [RelayCommand]
    private void Chart_OnMouseMove(MouseEventArgs e)
    {
        if (e.Handled) return;
        if (!_canMouseMoveToSelectPoint) return;
        if (e.LeftButton != MouseButtonState.Pressed) return;
        SetHighlightPointByPixelPosition(e.GetPosition(ChartBorder));
        e.Handled = true;
    }

    [RelayCommand]
    private void Chart_OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (e.Handled) return;
        _canMouseMoveToSelectPoint = false;
        e.Handled = true;
    }

    [RelayCommand]
    private void Chart_OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (e.Handled) return;
        _canMouseMoveToSelectPoint = true;
        SetHighlightPointByPixelPosition(e.GetPosition(ChartBorder));

        _isMark0 = true;
        MarkIntervalPointsData0 = null;
        MarkIntervalPointsData1 = null;

        e.Handled = true;
    }

    [RelayCommand]
    private void Chart_OnMouseRightButtonUp(MouseButtonEventArgs e)
    {
        // if (e.Handled) return;

        // e.Handled = true;
    }

    [RelayCommand]
    private void ShowRPoint()
    {
        ShowRPointVisibility = ShowRPointVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
    }

    [RelayCommand]
    private void Chart_OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        if (e.Handled) return;
        SetMarkedIntervalPointByPixelPosition(e.GetPosition(ChartBorder));

        CurrentHighlightPointData = null;

        e.Handled = true;
    }


    [RelayCommand]
    private void CreateMarkInterval()
    {
        if (MarkIntervalPointsData0 != null && MarkIntervalPointsData1 != null)
        {
            var time0 = MarkIntervalPointsData0.Time;
            var time1 = MarkIntervalPointsData1.Time;
            var label = MarkIntervalLabel;

            var beginTime = time0 > time1 ? time1 : time0;
            var endTime = time0 <= time1 ? time1 : time0;

            _ecgFileManager?.AddInterval(beginTime, endTime, label);

            MarkIntervalPointsData0 = null;
            MarkIntervalPointsData1 = null;
        }
    }


    [RelayCommand]
    private void RIntervalChartUpdated(ChartCommandArgs e)
    {
    }

    [RelayCommand]
    private void RIntervalPointerDown(PointerCommandArgs e)
    {
        _isDown = true;
    }

    [RelayCommand]
    private void RIntervalPointerMove(PointerCommandArgs args)
    {
        if (!_isDown) return;

        var chart = (ICartesianChartView<SkiaSharpDrawingContext>)args.Chart;
        var positionInData = chart.ScalePixelsToData(args.PointerPosition);
        var newValue = (long)(positionInData.X - ThumbTimeDuration / 2f);
        if (newValue < 0) newValue = 0;
        if (newValue > AllMilliSeconds - ThumbTimeDuration) newValue = AllMilliSeconds - ThumbTimeDuration;
        ThumbCurrentTime = newValue;
    }

    [RelayCommand]
    private void RIntervalPointerUp(PointerCommandArgs e)
    {
        _isDown = false;
    }


    [RelayCommand]
    private void PartRIntervalPointerDown(PointerCommandArgs e)
    {
        _isPartDown = true;
    }

    [RelayCommand]
    private void PartRIntervalPointerMove(PointerCommandArgs args)
    {
        if (!_isPartDown) return;

        var chart = (ICartesianChartView<SkiaSharpDrawingContext>)args.Chart;
        var positionInData = chart.ScalePixelsToData(args.PointerPosition);
        var newValue = (long)(positionInData.X - _partTimeDuration / 2f);
        if (newValue < ThumbCurrentTime) newValue = ThumbCurrentTime;
        if (newValue > ThumbCurrentTime + ThumbTimeDuration - _partTimeDuration)
            newValue = ThumbCurrentTime + ThumbTimeDuration - _partTimeDuration;
        _currentTimeAnimator.NoAnimateChangeTarget(newValue);
    }

    [RelayCommand]
    private void PartRIntervalPointerUp(PointerCommandArgs e)
    {
        _isPartDown = false;
    }


    [RelayCommand]
    private void RightMoveTime()
    {
        var newTime = CurrentTime + TimeInterval;
        newTime = Math.Clamp(newTime, 0, AllMilliSeconds - TimeInterval);
        _currentTimeAnimator.NoAnimateChangeTarget(newTime);
    }

    [RelayCommand]
    private void LeftMoveTime()
    {
        var newTime = CurrentTime - TimeInterval;
        newTime = Math.Clamp(newTime, 0, AllMilliSeconds - TimeInterval);
        _currentTimeAnimator.NoAnimateChangeTarget(newTime);
    }
}