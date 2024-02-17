namespace ECGPlatform;

public class Setting
{
    public string LocalDataDirectoryPath { get; set; }

    public Setting(string localDataDirectoryPath)
    {
        LocalDataDirectoryPath = localDataDirectoryPath;
    }
}