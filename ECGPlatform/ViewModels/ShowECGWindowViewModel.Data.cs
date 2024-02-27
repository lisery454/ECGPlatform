namespace ECGPlatform;

// update functions
public partial class ShowECGWindowViewModel
{
    private CancellationTokenSource? _updateWaveDataCts;

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
        HighlightPoints = result;
    }
}
