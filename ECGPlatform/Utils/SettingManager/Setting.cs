namespace ECGPlatform;

public class Setting
{
    public string LocalDataDirectoryPath { get; set; } = null!;

    public Setting(string localDataDirectoryPath)
    {
        LocalDataDirectoryPath = localDataDirectoryPath;
    }
    
    public Setting()
    {
    }
}