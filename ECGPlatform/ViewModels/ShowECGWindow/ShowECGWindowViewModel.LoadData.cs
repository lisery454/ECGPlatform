namespace ECGPlatform;

public partial class ShowECGWindowViewModel
{
    private CancellationTokenSource? _updateWaveDataCts;

    /// <summary>
    /// 加载波形数据
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async Task UpdateWaveData(CancellationToken cancellationToken)
    {
        if (_ecgFileManager == null) return;

        var result = new List<List<PointData>>();
        foreach (var i in Enumerable.Range(0, _ecgFileManager.WaveCount))
        {
            try
            {
                var data = await _ecgFileManager.GetRangedWaveDataAsync(i, CurrentTime, TimeInterval, 10,
                    cancellationToken);
                result.Add(data);
            }
            catch (OperationCanceledException)
            {
                // 正常取消获取数据操作
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
            }
        }

        if (cancellationToken.IsCancellationRequested) return;
        WaveDataCollection = result;
    }

    private CancellationTokenSource? _updateRPeaksDataCts;

    /// <summary>
    /// 加载R点数据
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async Task UpdateRPeaksData(CancellationToken cancellationToken)
    {
        if (_ecgFileManager == null) return;

        var result = new List<HighlightPointData>();

        try
        {
            result = await _ecgFileManager.GetRangedRPeaksAsync(CurrentTime, TimeInterval, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // 正常取消获取数据操作
        }
        catch (Exception e)
        {
            _logger.Error(e.ToString());
        }

        if (cancellationToken.IsCancellationRequested) return;


        RPeakPoints = result;
    }


    private CancellationTokenSource? _updateRIntervalDataCts;

    /// <summary>
    /// 加载R点间隔数据
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async Task UpdateRIntervalData(CancellationToken cancellationToken)
    {
        if (_ecgFileManager == null) return;

        var totalTime = _ecgFileManager.TotalTime;

        var result = new List<RIntervalData>();
        try
        {
            result = await _ecgFileManager.GetRIntervalDataAsync(0, totalTime, 2560, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // 正常取消获取数据操作
        }
        catch (Exception e)
        {
            _logger.Error(e.ToString());
        }

        RIntervalsData = result;
    }

    private CancellationTokenSource? _updatePartRIntervalDataCts;


    /// <summary>
    /// 加载Thumb的部分R点间隔数据
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async Task UpdatePartRIntervalData(CancellationToken cancellationToken)
    {
        if (_ecgFileManager == null) return;

        var result = new List<RIntervalData>();
        try
        {
            result = await _ecgFileManager.GetRIntervalDataAsync(ThumbCurrentTime, ThumbTimeDuration, 2560,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // 正常取消获取数据操作
        }
        catch (Exception e)
        {
            _logger.Error(e.ToString());
        }

        PartRIntervalsData = result;
    }

    private CancellationTokenSource? _updateSearchRPointDataCts;

    private async Task UpdateSearchRPointData(CancellationToken cancellationToken)
    {
        if (_ecgFileManager == null) return;
        IsSearching = true;

        var result = new List<HighlightPointData>();

        try
        {
            result = await _ecgFileManager.GetRangedRPeaksAsync(0, AllMilliSeconds, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // 正常取消获取数据操作
        }
        catch (Exception e)
        {
            _logger.Error(e.ToString());
        }

        if (cancellationToken.IsCancellationRequested) return;


        SearchPartRPointData =
            new ObservableCollection<HighlightPointData>(result.Where(data => data.Label == SearchRPeakLabel));

        IsSearching = false;
    }
}