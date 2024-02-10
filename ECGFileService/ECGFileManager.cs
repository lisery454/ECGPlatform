using SimpleUtils;
using YamlDotNet.Serialization;

namespace ECGFileService;

public class ECGFileManager : IDisposable
{
    public string IndexFilePath { get; set; }
    private ECGIndexFile IndexFile { get; set; } = null!;

    public ECGFileManager(string indexFilePath)
    {
        IndexFilePath = indexFilePath;
        ReadIndexFile(indexFilePath).Await(
            indexFile => { IndexFile = indexFile; },
            e => throw e);
    }

    public static async Task<ECGIndexFile> ReadIndexFile(string indexFilePath)
    {
        if (!File.Exists(indexFilePath))
        {
            throw new Exception($"this file: {indexFilePath} not exists");
        }

        var indexFileDirectoryPath = new FileInfo(indexFilePath).DirectoryName!;
        var text = await File.ReadAllTextAsync(indexFilePath);
        var ecgIndexFile = new Deserializer().Deserialize<ECGIndexFile>(text);
        ecgIndexFile.MainDataPath = Path.Combine(indexFileDirectoryPath, ecgIndexFile.MainDataPath);
        ecgIndexFile.RPeaksPath = Path.Combine(indexFileDirectoryPath, ecgIndexFile.RPeaksPath);
        ecgIndexFile.RPeaksModificationPath = Path.Combine(indexFileDirectoryPath, ecgIndexFile.RPeaksModificationPath);

        return ecgIndexFile;
    }


    public void Dispose()
    {
    }
}