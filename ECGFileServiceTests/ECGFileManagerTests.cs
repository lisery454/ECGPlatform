using ECGFileService;
using YamlDotNet.Serialization;

namespace ECGFileServiceTests;

public class ECGFileManagerTests
{
    [Test]
    public async Task CreateTest()
    {
        await GetECGFileManager();
        Assert.Pass();
    }

    [Test]
    public async Task GetRangedRPeaksAsyncTest()
    {
        using var ecgFileManager = await GetECGFileManager();
        _ = await ecgFileManager.GetRangedRPeaksAsync(0, ecgFileManager.TotalTime);
        Assert.Pass();
    }

    [Test]
    public async Task GetSingleWaveDataAsyncTest()
    {
        using var ecgFileManager = await GetECGFileManager();
        var pointData = await ecgFileManager.GetSingleWaveDataAsync(0, 1000);
        Assert.Pass($"{pointData.time} {pointData.value}");
    }

    [Test]
    public async Task GetRangedWaveDataAsyncTest()
    {
        using var ecgFileManager = await GetECGFileManager();
        var pointData = await ecgFileManager.GetRangedWaveDataAsync(0, 0, 1000);
        Assert.Pass($"{pointData.Count}");
    }

    [Test]
    public async Task GetRIntervalDataAsyncTest()
    {
        using var ecgFileManager = await GetECGFileManager();
        var pointData = await ecgFileManager.GetRIntervalDataAsync(0, ecgFileManager.TotalTime, 2560);
        Assert.Pass($"{pointData.Count}");
    }

    [Test]
    public async Task WaveCountTest()
    {
        using var ecgFileManager = await GetECGFileManager();
        var waveCount = ecgFileManager.WaveCount;
        Assert.Pass(waveCount.ToString());
    }

    [Test]
    public async Task TotalTimeTest()
    {
        using var ecgFileManager = await GetECGFileManager();
        var totalTime = ecgFileManager.TotalTime;
        Assert.Pass(totalTime.ToString());
    }

    [Test]
    public async Task FrameToTimeWaveTest()
    {
        using var ecgFileManager = await GetECGFileManager();
        var time = ecgFileManager.FrameToTimeWave(1000);
        Assert.Pass(time.ToString());
    }


    private async Task<ECGFileManager> GetECGFileManager()
    {
        const string filePath = @"E:\Data\毕业设计\data\data_1\index.yaml";
        var index = await new ReadIndexFileService(new Deserializer()).Read(filePath);
        return new ECGFileManager(index);
    }
}