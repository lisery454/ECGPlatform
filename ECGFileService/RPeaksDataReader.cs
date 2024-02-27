using System.Diagnostics;
using System.Text;
using SimpleUtils;

namespace ECGFileService;

public class RPeaksDataReader : IDisposable
{
    private readonly string _rPeaksPath;
    private readonly int _rPeaksWidth;
    private readonly ObjectPool<BufferedFileStream> _fileStreamPool;
    private readonly int _frequency;
    private Dictionary<int, RPeakUnit> _cache;

    private int AllRPeaksNum { get; }

    public RPeaksDataReader(string rPeaksPath, int frequency, int rPeaksWidth)
    {
        _rPeaksPath = rPeaksPath;
        _frequency = frequency;
        _fileStreamPool = new ObjectPool<BufferedFileStream>(
            () => new BufferedFileStream(
                new FileStream(rPeaksPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite), 96), 5);
        _rPeaksWidth = rPeaksWidth;
        AllRPeaksNum = GetAllRPeaksNum();
        _cache = new Dictionary<int, RPeakUnit>();
    }

    public async Task<List<RPeakUnit>> GetDataAsync(long beginTime, long lastTime,
        CancellationToken cancellationToken = default)
    {
        var result = await GetOriginalDataAsync(beginTime, lastTime, cancellationToken);
        return result;
    }

    private async Task<List<RPeakUnit>> GetOriginalDataAsync(long beginTime, long lastTime,
        CancellationToken cancellationToken = default)
    {
        var endTime = beginTime + lastTime;
        var beginIndex = await FindFirstIndexBiggerEqualThenAsync(beginTime, cancellationToken);
        var endIndex = await FindFirstIndexBiggerEqualThenAsync(endTime, cancellationToken);
        var res = await RangeValueAtAsync(beginIndex, endIndex, cancellationToken);

        return res;
    }

    private async Task<List<RPeakUnit>> RangeValueAtAsync(int beginIndex, int endIndex,
        CancellationToken cancellationToken = default)
    {
        if (beginIndex >= GetAllRPeaksNum() || endIndex >= GetAllRPeaksNum() || beginIndex > endIndex)
            throw new Exception("index err");
        var result = new List<RPeakUnit>();
        var lastIndex = endIndex - beginIndex;
        const int bufferSize = 1024;
        var buffer = new byte[bufferSize];
        await using var fileStream =
            new FileStream(_rPeaksPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        fileStream.Seek(beginIndex * _rPeaksWidth, SeekOrigin.Begin);
        var stringBuilder = new StringBuilder();
        int bytesRead, i;
        char ch;
        long currentIndex = 0;
        while ((bytesRead = await fileStream.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken)) != 0)
        {
            if (bytesRead == 0) break;
            for (i = 0; i < bytesRead; i++)
            {
                ch = (char)buffer[i];
                if (ch == '\n')
                {
                    if (currentIndex < lastIndex) ParseStrToData(stringBuilder, result);
                    else return result;
                }
                else if (ch != '\r') stringBuilder.Append(ch);
            }
        }

        if (currentIndex < lastIndex) ParseStrToData(stringBuilder, result);

        return result;

        void ParseStrToData(StringBuilder sb, List<RPeakUnit> tmd)
        {
            var strings = sb.ToString().Split(';');
            if (strings.Length != 2)
            {
                Debug.WriteLine(sb);
            }
            else if (long.TryParse(strings[0], out var frame) && int.TryParse(strings[1], out var id))
            {
                var value = new RPeakUnit(id, FrameToTime(frame));
                tmd.Add(value);
            }
            else
            {
                Debug.WriteLine("读取RPeaksData：转换失败");
            }

            sb.Clear();
            currentIndex++;
        }
    }

    private async Task<int> FindFirstIndexBiggerEqualThenAsync(long time,
        CancellationToken cancellationToken = default)
    {
        var begin = 0;
        var end = AllRPeaksNum;
        while (begin <= end - 1)
        {
            if (begin == end - 1)
            {
                var rPeakUnitValue = await ValueAtAsync(begin, cancellationToken);
                return rPeakUnitValue.Time >= time ? begin : end;
            }

            var middle = (end + begin) / 2;

            var middleValue = await ValueAtAsync(middle, cancellationToken);
            if (middleValue.Time < time)
                begin = middle + 1;
            else if (middleValue.Time > time)
                end = middle;
            else
                return middle;
        }

        return end;
    }

    private int GetAllRPeaksNum()
    {
        var fileInfo = new FileInfo(_rPeaksPath);
        return (int)((float)fileInfo.Length / _rPeaksWidth);
    }

    private async Task<RPeakUnit> ValueAtAsync(int index, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(index, out var unit)) return unit;
        
        if (index >= GetAllRPeaksNum()) throw new Exception("index >= all indexes");
        var fs = _fileStreamPool.Get();

        var result = await fs.ReadAsync(index * _rPeaksWidth, _rPeaksWidth, cancellationToken);

        _fileStreamPool.Release(fs);
        var strings = result.Split(';');

        if (long.TryParse(strings[0], out var frame) && int.TryParse(strings[1], out var id))
        {
            var res = new RPeakUnit(id, FrameToTime(frame));
            _cache.Add(index, res);
            return res;
        }

        throw new Exception("parse str fail!");
    }

    private long FrameToTime(long frame)
    {
        return frame * 1000 / _frequency;
    }

    private long TimeToFrame(long time)
    {
        return time * _frequency / 1000;
    }

    public void Dispose()
    {
        _fileStreamPool.Foreach(fs => { fs.Dispose(); });
    }
}