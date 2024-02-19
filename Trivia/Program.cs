using System.Diagnostics;
using ECGFileService;

var waveDataReader = new WaveDataReader(@"E:\Data\毕业设计\data\data_1\1.txt", 250, 7, 0.0024f);


var stopwatch = new Stopwatch();
stopwatch.Start();
var data = await waveDataReader.GetDataParallelAsync(0, waveDataReader.TotalTime, 10);

stopwatch.Stop();

Console.WriteLine($"Total Use Time: {stopwatch.ElapsedMilliseconds}");
Console.WriteLine($"Read Data Count: {data.Count}");
Console.WriteLine($"Read Data Time: {waveDataReader.TotalTime}");