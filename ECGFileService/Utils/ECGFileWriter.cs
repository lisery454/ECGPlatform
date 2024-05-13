using YamlDotNet.Serialization;

namespace ECGFileService;

public static class ECGFileWriter
{
    private static readonly IndexFileService IndexFileService = new(new Deserializer(), new Serializer());

    public static async Task Write(string directoryPath, int fragmentId, List<int> data, int rate,
        List<RPeakUnit> rPeakUnits)
    {
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        var fragmentDirPath = Path.Combine(directoryPath, $"{fragmentId}");

        if (!Directory.Exists(fragmentDirPath))
            Directory.CreateDirectory(fragmentDirPath);
        else return;

        var ecgIndex = new ECGIndex
        {
            Title = $"{fragmentId}",
            WaveDataFrequency = rate,
            RPeaksFrequency = 250,
            WaveDataYFactor = 0.0024f,
            RPeaksPath = "r_peaks.txt",
            IntervalFilePath = "int.txt",
            RPeaksModificationPath = "mod.txt",
            WaveDataPaths = new List<string> { "1.txt" },
        };

        await using var fileStream = new StreamWriter(new FileStream(Path.Combine(fragmentDirPath, "1.txt"),
            FileMode.OpenOrCreate,
            FileAccess.ReadWrite));

        foreach (var num in data)
        {
            await fileStream.WriteAsync($"{num,6:00000};");
            // await fileStream.WriteAsync($"{num:D6}" + ";");
        }

        ecgIndex.WaveDataWidth = 7;

        await using var fileStream2 = new StreamWriter(new FileStream(Path.Combine(fragmentDirPath, "r_peaks.txt"),
            FileMode.OpenOrCreate,
            FileAccess.ReadWrite));

        foreach (var rPeakUnit in rPeakUnits)
        {
            var frame = rPeakUnit.Time * rate / 1000;
            var labelId = RPeakUnit.FromLabelToId(rPeakUnit.Label);
            await fileStream2.WriteLineAsync(frame.ToString().PadLeft(12, '0') + ";" +
                                             labelId.ToString().PadLeft(2, '0'));
        }

        ecgIndex.RPeaksWidth = 17;


        await IndexFileService.Write(ecgIndex, Path.Combine(fragmentDirPath, "index.yaml"));
    }
}