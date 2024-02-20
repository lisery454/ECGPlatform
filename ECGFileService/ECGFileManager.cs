namespace ECGFileService;

public class ECGFileManager : IDisposable
{
    private ECGIndex Index { get; set; }

    public readonly List<WaveDataReader> waveDataReaders = new();

    public ECGFileManager (ECGIndex index)
    {
        Index = index;
        waveDataReaders.ForEach(reader => reader.Dispose());
        foreach (var waveDataPath in Index.WaveDataPaths)
        {
            waveDataReaders.Add(new WaveDataReader(waveDataPath, Index.WaveDataFrequency, Index.WaveDataWidth,
                Index.WaveDataYFactor));
        }
    }
    
    public void Dispose()
    {
        waveDataReaders.ForEach(reader => reader.Dispose());
    }
}