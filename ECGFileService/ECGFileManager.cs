using System.Diagnostics;
using SimpleUtils;

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

        _rPeaksDataReader = new RPeaksDataReader(Index.RPeaksPath, Index.RPeaksFrequency, Index.RPeaksWidth,
            Index.RPeaksModificationPath);
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

    public async Task<List<PointData>> GetRangedWaveDataAsync(int index, long beginTime, long lastTime, int threadCount = 10,
        CancellationToken cancellationToken = default)
    {
        return await _waveDataReaders[index].GetDataParallelAsync(beginTime, lastTime, threadCount, cancellationToken);
    }

    public async Task<List<HighlightPointData>> GetRangedRPeaksAsync(long beginTime, long lastTime,
        CancellationToken cancellationToken = default)
    {
        var rPeakUnits = await _rPeaksDataReader.GetDataAsync(beginTime, lastTime, cancellationToken);

        var result = new List<HighlightPointData>();
        foreach (var rPeakUnit in rPeakUnits)
        {
            var values = new List<float>();
            for (var i = 0; i < WaveCount; i++)
            {
                values.Add((await GetSingleWaveDataAsync(i, rPeakUnit.Time, cancellationToken)).value);
            }

            result.Add(new HighlightPointData(rPeakUnit.Time, values, rPeakUnit.Label));
        }

        return result;
    }

    public async Task AddRPeakPointAsync(long time, RPeakLabel label)
    {
        await _rPeaksDataReader.AddRPeakPointAsync(time, label);
    }

    public async Task DeleteRPeak(long time)
    {
        await _rPeaksDataReader.DeleteRPeakPointAsync(time);
    }

    public async Task UpdateRPeakPointLabel(long time, RPeakLabel oldLabel, RPeakLabel newLabel)
    {
        await _rPeaksDataReader.UpdateRPeakPointLabel(time, oldLabel, newLabel);
    }

    public async Task<List<RIntervalData>> GetRIntervalDataAsync(long beginTime, long lastTime,
        int dataCount = 2560,
        CancellationToken cancellationToken = default)
    {
        return await _rPeaksDataReader.GetRIntervalDataAsync(beginTime, lastTime, dataCount, cancellationToken);
    }
}