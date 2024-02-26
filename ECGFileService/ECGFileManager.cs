namespace ECGFileService;

public class ECGFileManager : IDisposable
{
    private ECGIndex Index { get; set; }

    private readonly List<WaveDataReader> _waveDataReaders;
    private readonly RPeaksDataReader _rPeaksDataReader;

    public ECGFileManager(ECGIndex index)
    {
        Index = index;
        _waveDataReaders = new List<WaveDataReader>();
        foreach (var waveDataPath in Index.WaveDataPaths)
        {
            _waveDataReaders.Add(new WaveDataReader(waveDataPath, Index.WaveDataFrequency, Index.WaveDataWidth,
                Index.WaveDataYFactor));
        }

        _rPeaksDataReader = new RPeaksDataReader(Index.RPeaksPath, Index.RPeaksFrequency, Index.RPeaksWidth);
    }

    public void Dispose()
    {
        _waveDataReaders.ForEach(reader => reader.Dispose());
    }

    public int WaveCount => _waveDataReaders.Count;

    public long TotalTime => _waveDataReaders.First().TotalTime;


    public async Task<PointData> GetSingleWaveDataAsync(int index, long time,
        CancellationToken cancellationToken = default)
    {
        return await _waveDataReaders[index].ValueAtAsync(time, cancellationToken);
    }

    public async Task<List<PointData>> GetRangedWaveDataAsync(int index, long beginTime, long lastTime, int count = 10,
        CancellationToken cancellationToken = default)
    {
        return await _waveDataReaders[index].GetDataParallelAsync(beginTime, lastTime, count, cancellationToken);
    }

    public async Task<List<HighlightPoint>> GetRangedRPeaksAsync(long beginTime, long lastTime,
        CancellationToken cancellationToken = default)
    {
        var rPeakUnits = await _rPeaksDataReader.GetDataAsync(beginTime, lastTime, cancellationToken);

        return (from rPeakUnit in rPeakUnits
            let values = Enumerable.Range(0, WaveCount)
                .Select(async i => await GetSingleWaveDataAsync(i, rPeakUnit.Time, cancellationToken))
                .Select(task => task.Result.value)
                .ToList()
            select new HighlightPoint(rPeakUnit.Time, values, rPeakUnit.Id)).ToList();
    }
}