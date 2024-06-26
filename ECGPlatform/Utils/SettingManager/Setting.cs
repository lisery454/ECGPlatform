﻿namespace ECGPlatform;

public class Setting
{
    public string LocalDataDirectoryPath { get; set; } = null!;
    public LanguageType LanguageType { get; set; } = LanguageType.CHINESE;
    public ThemeType ThemeType { get; set; } = ThemeType.LIGHT;

    public Setting(string localDataDirectoryPath)
    {
        LocalDataDirectoryPath = localDataDirectoryPath;
    }

    public Setting()
    {
    }
}