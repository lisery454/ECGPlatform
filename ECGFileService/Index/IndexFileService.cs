using YamlDotNet.Serialization;

namespace ECGFileService;

public class IndexFileService
{
    private readonly IDeserializer _deserializer;
    private readonly ISerializer _serializer;

    public IndexFileService(IDeserializer deserializer, ISerializer serializer)
    {
        _deserializer = deserializer;
        _serializer = serializer;
    }

    public async Task Write(ECGIndex index, string indexFilePath)
    {
        var text = _serializer.Serialize(index);
        await using var streamWriter =
            new StreamWriter(new FileStream(indexFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite));

        await streamWriter.WriteAsync(text);
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
        ecgIndex.IntervalFilePath = Path.Combine(indexFileDirectoryPath, ecgIndex.IntervalFilePath);

        return ecgIndex;
    }
}