using ECGFileService;
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
        var rangedRPeaksAsync = await ecgFileManager.GetRangedRPeaksAsync(0, 10000);
        Assert.Pass(rangedRPeaksAsync.Count.ToString());
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

    private async Task<ECGFileManager> GetECGFileManager()
    {
        const string filePath = @"E:\Data\毕业设计\data\data_1\index.yaml";
        var index = await new ReadIndexFileService(new Deserializer()).Read(filePath);
        return new ECGFileManager(index);
    }
}