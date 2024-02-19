using SimpleUtils;

namespace ECGFileService;

public class ECGFileManager : IDisposable
{
    private ECGIndex Index { get; set; } = null!;

    public void Load(ECGIndex index)
    {
        Index = index;
        Index.Debug();
    }


    public void Dispose()
    {
    }
}