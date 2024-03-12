using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// var waveDataReader = new WaveDataReader(@"E:\Data\毕业设计\data\data_1\1.txt", 250, 7, 0.0024f);

//
// var stopwatch = new Stopwatch();
// stopwatch.Start();
// var data = await waveDataReader.GetDataParallelAsync(0, waveDataReader.TotalTime, 10);
//
// stopwatch.Stop();
//
// Console.WriteLine($"Total Use Time: {stopwatch.ElapsedMilliseconds}");
// Console.WriteLine($"Read Data Count: {data.Count}");
// Console.WriteLine($"Read Data Time: {waveDataReader.TotalTime}");


// const string filePath = @"E:\Data\毕业设计\data\data_1\1.txt";
// var stopwatch = new Stopwatch();
// stopwatch.Start();
// for (var i = 0; i <= 10; i++)
// {
//     var array = new byte[1024];
//     var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
// }
// stopwatch.Stop();
//
// Console.WriteLine($"Total Use Time: {stopwatch.ElapsedMilliseconds}");


using var fs = new StreamReader(@"C:\Users\lxl11\Desktop\data.json");

var jObject = JObject.Parse(fs.ReadToEnd());

// string json = "{\"ID\":1,\"Name\":\"张三\",\"Birthday\":\"2000-01-02T00:00:00\",\"IsVIP\":true,\"Account\":12.34,\"Favorites\":[\"吃饭\",\"睡觉\"],\"Remark\":null}";
//
// JObject obj = JObject.Parse(json);

Console.WriteLine("Hello");
