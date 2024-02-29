namespace ECGPlatform;

public partial class ShowECGWindowViewModel
{
    [RelayCommand]
    private async Task LoadData()
    {
        // 加载文件
        _ecgFileManager?.Dispose();
        _ecgFileManager = new ECGFileManager(EcgIndex!);

        // 加载时间数据
        AllMilliSeconds = _ecgFileManager.TotalTime;
        CurrentTime = 0;
        TimeInterval = 5000;

        // 加载波形数据
        await UpdateWaveData(CtsUtils.Refresh(ref _updateWaveDataCts).Token);

        IsLoadingData = false;
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
    }

    [RelayCommand]
    private async Task ChartUpdated()
    {
        // HideAllRPeakPoints();
        await UpdateRPeaksData(CtsUtils.Refresh(ref _updateRPeaksDataCts).Token);
    }

    [RelayCommand]
    private void UpdateRPoint()
    {
    }

    [RelayCommand]
    private void DeleteRPoint()
    {
    }

    [RelayCommand]
    private void CreateRPoint()
    {
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
        e.Handled = true;
    }
}