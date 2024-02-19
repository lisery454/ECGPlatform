namespace ECGPlatform;

public class LocalECGDataItem
{
    public string Title { get; set; }

    public string IndexFilePath { get; set; }

    public ECGIndex ECGIndex { get; set; }

    public LocalECGDataItem(ECGIndex ecgIndex, string indexFilePath, string title)
    {
        ECGIndex = ecgIndex;
        IndexFilePath = indexFilePath;
        Title = title;
    }
}