using YamlDotNet.Serialization;

namespace ECGFileService;

public class ReadIndexFileService
{
    private readonly IDeserializer _deserializer;

    public ReadIndexFileService(IDeserializer deserializer)
    {
        _deserializer = deserializer;
    }

    /// <summary>
    /// 读取index文件
    /// </summary>
    /// <param name="indexFilePath">index文件路径</param>
    /// <returns>ECGIndexFile</returns>
    /// <exception cref="Exception"></exception>
    public async Task<ECGIndex> Read(string indexFilePath)
    {
        if (!File.Exists(indexFilePath))
        {
            throw new Exception($"this file: {indexFilePath} not exists");
        }

        var indexFileDirectoryPath = new FileInfo(indexFilePath).DirectoryName!;
        var text = await File.ReadAllTextAsync(indexFilePath);
        var ecgIndex = _deserializer.Deserialize<ECGIndex>(text);
        ecgIndex.WaveDataPaths =
            ecgIndex.WaveDataPaths.Select(path => Path.Combine(indexFileDirectoryPath, path)).ToList();
        ecgIndex.RPeaksPath = Path.Combine(indexFileDirectoryPath, ecgIndex.RPeaksPath);
        ecgIndex.RPeaksModificationPath = Path.Combine(indexFileDirectoryPath, ecgIndex.RPeaksModificationPath);

        return ecgIndex;
    }
}