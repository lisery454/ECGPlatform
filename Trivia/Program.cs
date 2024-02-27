using System.Diagnostics;
using ECGFileService;
using SimpleUtils;

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


async Task Foo()
{
    await Task.Delay(2000);
    Console.WriteLine("1");
}


Foo().Await();

Console.WriteLine("2");

Console.ReadLine();