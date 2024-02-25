using System.Diagnostics;
using System.Text;
using SimpleUtils;

namespace ECGFileService;

public class WaveDataReader : IDisposable
{
    private readonly string _filePath;
    private readonly int _frequency;
    private readonly int _width;
    private readonly float _yFactor;
    private readonly ObjectPool<BufferedFileStream> _fileStreamPool;
    private const int MainFileStreamBufferSize = 2048;
    private const int FileStreamPoolBufferSize = 96;
    public long TotalTime { get; private set; }

    private bool _isCached;
    private bool _isCachedOk;
    private bool CanUseCache => _isCached && _isCachedOk;
    private List<float> _waveCache;

    public WaveDataReader(string filePath, int frequency, int width, float yFactor, bool isCached = false)
    {
        _filePath = filePath;
        _frequency = frequency;
        _width = width;
        _yFactor = yFactor;
        _isCached = isCached;
        _isCachedOk = false;
        _waveCache = new List<float>();
        TotalTime = new FileInfo(_filePath).Length / _width / _frequency * 1000;
        _fileStreamPool = new ObjectPool<BufferedFileStream>(
            () => new BufferedFileStream(
                new FileStream(_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite),
                FileStreamPoolBufferSize), 5);

        if (isCached)
        {
            UpdateCache().Await(() =>
            {
                _isCachedOk = true;
                "update cache finished!".Debug();
            }, exception => throw exception);
        }
    }

    private async Task UpdateCache()
    {
        var dataParallelAsync = await GetDataParallelAsync(0, TotalTime, count: 10);
        _waveCache = dataParallelAsync.Select(data => data.value).ToList();
    }


    public void Dispose()
    {
        _fileStreamPool.Foreach(fs => { fs.Dispose(); });
    }


    /// <summary>
    /// 把时间转化成帧数
    /// </summary>
    /// <param name="time">时间单位是ms</param>
    /// <returns>时间对应的帧数</returns>
    private long TimeToFrame(long time)
    {
        return (long)(time / 1000f * _frequency);
    }

    /// <summary>
    /// 把帧数转化成时间
    /// </summary>
    /// <param name="frame">帧数</param>
    /// <returns>帧数对应的时间</returns>
    private long FrameToTime(long frame)
    {
        return (long)(frame * 1000f / _frequency);
    }

    /// <summary>
    /// 在某个时间的波形数据
    /// </summary>
    /// <param name="time">时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns>波形数据</returns>
    /// <exception cref="Exception">读取数据异常</exception>
    public async Task<PointData> ValueAtAsync(long time, CancellationToken cancellationToken = default)
    {
        if (CanUseCache)
        {
            var frame = TimeToFrame(time);
            var value = _waveCache[(int)frame];
            return new PointData(time, value);
        }
        else
        {
            var fs = _fileStreamPool.Get();
            var s = await fs.ReadAsync(TimeToFrame(time) * _width, _width, cancellationToken);
            _fileStreamPool.Release(fs);
            var strings = s.Split(';');
            if (float.TryParse(strings[0], out var result))
                return new PointData(time, result * _yFactor);

            throw new Exception("转换失败");
        }
    }

    /// <summary>
    /// 获取一段时间内的波形数据
    /// </summary>
    /// <param name="beginTime">开始的时间，单位是ms</param>
    /// <param name="lastTime">持续的时间，单位是ms</param>
    /// <param name="count">线程数</param>
    /// <param name="cancellationToken"></param>
    /// <returns>这段时间内的波形数据</returns>
    public async Task<List<PointData>> GetDataParallelAsync(long beginTime, long lastTime, int count = 10,
        CancellationToken cancellationToken = default)
    {
        if (CanUseCache)
        {
            var beginFrame = (int)TimeToFrame(beginTime);
            var lastFrame = (int)TimeToFrame(lastTime);

            return _waveCache.GetRange(beginFrame, lastFrame)
                .Select((value, index) => new PointData(FrameToTime(index), value)).ToList();
        }
        else
        {
            var subLastTime = lastTime * 1f / count;
            var beginFrame = new List<long>();
            for (var i = 0; i < count; i++) beginFrame.Add(TimeToFrame((long)(beginTime + subLastTime * i)));
            var lastFrame = new List<long>();
            for (var i = 0; i < count - 1; i++) lastFrame.Add(beginFrame[i + 1] - beginFrame[i]);
            lastFrame.Add(TimeToFrame(lastTime) + beginFrame[0] - beginFrame[count - 1]);

            var tasks = Enumerable.Range(0, count)
                .Select(i => GetDataSubParallelAsync(beginFrame[i], lastFrame[i], cancellationToken)).ToList();

            await Task.WhenAll(tasks);

            var result = tasks.Aggregate(new List<PointData>(), (data, task) =>
            {
                data.AddRange(task.Result);
                return data;
            });
            return result;
        }
    }

    private async Task<List<PointData>> GetDataSubParallelAsync(long beginFrame, long lastFrame,
        CancellationToken cancellationToken = default)
    {
        var pointData = new List<PointData>();
        var beginTime = FrameToTime(beginFrame);
        var array = new byte[MainFileStreamBufferSize];
        var waveFileStream = new FileStream(_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        var stringBuilder = new StringBuilder();
        int bytesRead, i;
        long currentFrame = 0;
        char ch;

        waveFileStream.Seek(beginFrame * _width, SeekOrigin.Begin);
        while ((bytesRead =
                   await waveFileStream.ReadAsync(array.AsMemory(0, MainFileStreamBufferSize), cancellationToken)) > 0)
        {
            if (bytesRead == 0) break;
            for (i = 0; i < bytesRead; i++)
            {
                ch = (char)array[i];
                if (ch == ';')
                {
                    if (currentFrame < lastFrame) ParseStrToData();
                    else return pointData;
                }
                else stringBuilder.Append(ch);
            }
        }

        if (currentFrame < lastFrame) ParseStrToData();
        await waveFileStream.DisposeAsync();

        return pointData;

        void ParseStrToData()
        {
            var strings = stringBuilder.ToString().Split(';');
            if (float.TryParse(strings[0], out var result))
            {
                var time = FrameToTime(currentFrame) + beginTime;
                pointData.Add(new PointData(time, result * _yFactor));
            }
            else
            {
                throw new Exception("浮点数转换失败");
            }

            stringBuilder.Clear();
            currentFrame++;
        }
    }
}