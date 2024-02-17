namespace ECGPlatform;

public class SettingManager : ISettingManager
{
    private readonly string _settingPath;
    private readonly string _defaultLocalDataPath;

    private Setting _currentSetting;

    public Setting CurrentSetting
    {
        get => _currentSetting;
        set
        {
            _currentSetting = value;
            Save();
        }
    }

    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;
    private readonly ILogger _logger;

    public SettingManager(ISerializer serializer, IDeserializer deserializer, ILogger logger,
        ProgramConstants programConstants)
    {
        _serializer = serializer;
        _deserializer = deserializer;
        _logger = logger;
        _settingPath = programConstants.SettingPath;
        _defaultLocalDataPath = programConstants.DefaultLocalDataDirectoryPath;

        CheckIfSettingFileExists();
        try
        {
            _logger.Debug(File.ReadAllText(_settingPath));
            _currentSetting = _deserializer.Deserialize<Setting>(File.ReadAllText(_settingPath));
            _logger.Information("Load Setting Success.");
        }
        catch (Exception e)
        {
            _logger.Warning($"Load Setting Fail. Exception: {e}");
            _currentSetting = new Setting(_defaultLocalDataPath);
            _logger.Information("Create Default Setting.");
            Save();
        }
    }

    public void Save()
    {
        CheckIfSettingFileExists();
        var serialize = _serializer.Serialize(CurrentSetting);
        using var sw = new StreamWriter(_settingPath);
        sw.WriteLine(serialize);
        _logger.Information("Save Setting Success.");
    }

    private void CheckIfSettingFileExists()
    {
        if (File.Exists(_settingPath)) return;

        using var fileStream = File.Create(_settingPath);
        using var sw = new StreamWriter(fileStream);
        var serialize = _serializer.Serialize(new Setting(_defaultLocalDataPath));
        sw.WriteLine(serialize);
    }
}