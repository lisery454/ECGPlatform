namespace ECGFileService;


public class ECGIndexFile
{
    public string Title { get; set; } = string.Empty;
    public string MainDataPath { get; set; } = string.Empty;
    public int MainDataWidth { get; set; }
    public int MainDataFrequency { get; set; }
    public float MainDataYFactor { get; set; }
    public string RPeaksPath { get; set; } = string.Empty;
    public int RPeaksWidth { get; set; }
    public int RPeaksFrequency { get; set; }
    public string RPeaksModificationPath { get; set; } = string.Empty;
}