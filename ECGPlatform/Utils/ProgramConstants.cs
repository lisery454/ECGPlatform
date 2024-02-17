namespace ECGPlatform;

public class ProgramConstants
{
    public string ApplicationPath
    {
        get
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ECGPlatform");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }

    public string SettingPath => Path.Combine(ApplicationPath, "setting.yaml");

    public string LogPath => Path.Combine(ApplicationPath, "log.txt");
    
    public string DefaultLocalDataDirectoryPath => Path.Combine(ApplicationPath, "data");
}