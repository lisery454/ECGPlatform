namespace ECGPlatform;

public interface ISettingManager
{
    void Save();
    Setting CurrentSetting { get; set; }
}