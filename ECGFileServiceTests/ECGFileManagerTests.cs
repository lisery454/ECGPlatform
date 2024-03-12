using System.Diagnostics;
using ECGFileService;
using SimpleUtils;
using YamlDotNet.Serialization;

namespace ECGFileServiceTests;

public class ECGFileManagerTests
{
    [Test]
    public async Task Test1()
    {
        await GetECGFileManager();
        Assert.Pass();
    }

    [Test]
    public async Task Test2()
    {
        using var ecgFileManager = await GetECGFileManager();

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var rangedRPeaksAsync = await ecgFileManager.GetRangedRPeaksAsync(0, ecgFileManager.TotalTime);
        stopwatch.Stop();
        Console.WriteLine($"time: {stopwatch.ElapsedMilliseconds} ");

        Assert.Pass();
    }

    [Test]
    public async Task Test3()
    {
        using var ecgFileManager = await GetECGFileManager();
        var pointData = await ecgFileManager.GetSingleWaveDataAsync(0, 1000);
        Assert.Pass($"{pointData.time} {pointData.value}");
    }

    [Test]
    public async Task Test4()
    {
        using var ecgFileManager = await GetECGFileManager();
        var pointData = await ecgFileManager.GetRangedWaveDataAsync(0, 0, 1000);
        Assert.Pass($"{pointData.Count}");
    }

    [Test]
    public async Task Test5()
    {
        using var ecgFileManager = await GetECGFileManager();
        var pointData = await ecgFileManager.GetRIntervalDataAsync(0, ecgFileManager.TotalTime, 2560);
        Assert.Pass($"{pointData.Count}");
    }


    private async Task<ECGFileManager> GetECGFileManager()
    {
        const string filePath = @"E:\Data\毕业设计\data\data_1\index.yaml";
        var index = await new ReadIndexFileService(new Deserializer()).Read(filePath);
        return new ECGFileManager(index);
    }
}