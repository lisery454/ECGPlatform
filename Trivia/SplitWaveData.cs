using System.Text;

namespace Trivia;

public static class SplitWaveData
{
    public static void Run()
    {
        Split(@"E:\Data\毕业设计\data\data_1\data.txt");
    }

    public static void Split(string waveFilePath)
    {
        var directoryName = new FileInfo(waveFilePath).Directory!.FullName;
        using var fileStream = new FileStream(waveFilePath, FileMode.Open);
        using var fileStream0 =
            new StreamWriter(new FileStream(Path.Combine(directoryName, "1.txt"), FileMode.OpenOrCreate));
        using var fileStream1 =
            new StreamWriter(new FileStream(Path.Combine(directoryName, "2.txt"), FileMode.OpenOrCreate));
        using var fileStream2 =
            new StreamWriter(new FileStream(Path.Combine(directoryName, "3.txt"), FileMode.OpenOrCreate));
        const int bufferSize = 2048;
        var buffer = new byte[bufferSize];
        var stringBuilder = new StringBuilder();
        int readBytes;
        while ((readBytes = fileStream.Read(buffer)) > 0)
        {
            for (var i = 0; i < readBytes; i++)
            {
                var ch = (char)buffer[i];
                if (ch == '\n')
                {
                    var strings = stringBuilder.ToString().Split(';');
                    fileStream0.Write(strings[0] + ";");
                    fileStream1.Write(strings[1] + ";");
                    fileStream2.Write(strings[2] + ";");
                    stringBuilder.Clear();
                }
                else if (ch != '\r') stringBuilder.Append(ch);
            }
        }
    }
}