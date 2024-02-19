namespace ECGFileService;

public class ECGIndex
{
    public string Title { get; set; } = string.Empty;

    public List<string> WaveDataPaths { get; set; } = new();
    
    public int WaveDataWidth { get; set; }
    public int WaveDataFrequency { get; set; }
    public float WaveDataYFactor { get; set; }

    public string RPeaksPath { get; set; } = string.Empty;
    public int RPeaksWidth { get; set; }
    public int RPeaksFrequency { get; set; }

    public string RPeaksModificationPath { get; set; } = string.Empty;
}