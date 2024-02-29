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
    private readonly Dictionary<int, RPeakUnit> _cache;
    private readonly string _modificationPath;
    private readonly List<IRPeaksModifyEntry> _entries;

    private int AllRPeaksNum { get; }

    public RPeaksDataReader(string rPeaksPath, int frequency, int rPeaksWidth, string modificationPath)
    {
        _rPeaksPath = rPeaksPath;
        _frequency = frequency;
        _modificationPath = modificationPath;
        _fileStreamPool = new ObjectPool<BufferedFileStream>(
            () => new BufferedFileStream(
                new FileStream(rPeaksPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite), 96), 5);
        _rPeaksWidth = rPeaksWidth;
        AllRPeaksNum = GetAllRPeaksNum();
        _cache = new Dictionary<int, RPeakUnit>();
        _entries = new List<IRPeaksModifyEntry>();
        InitEntries();
    }

    public async Task<List<RPeakUnit>> GetDataAsync(long beginTime, long lastTime,
        CancellationToken cancellationToken = default)
    {
        var result = await GetOriginalDataAsync(beginTime, lastTime, cancellationToken);
        return GetAfterModifyData(beginTime, lastTime, result);
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
                var value = new RPeakUnit((RPeakLabel)id, FrameToTime(frame));
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
            var res = new RPeakUnit((RPeakLabel)id, FrameToTime(frame));
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

    private void InitEntries()
    {
        using var modificationFileStream =
            new FileStream(_modificationPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        modificationFileStream.Seek(0, SeekOrigin.Begin);
        using var sr = new StreamReader(modificationFileStream);
        string? line;
        long frame;
        int labelNum;
        int oldLabelNum;
        int newLabelNum;
        RPeaksModifyEntryType rPeaksModifyEntryType;
        string[] strings;
        while (!string.IsNullOrWhiteSpace(line = sr.ReadLine()))
        {
            strings = line.Split(';');
            rPeaksModifyEntryType = IRPeaksModifyEntry.StrToType(strings[0]);
            switch (rPeaksModifyEntryType)
            {
                case RPeaksModifyEntryType.CREATE:
                    frame = long.Parse(strings[1]);
                    labelNum = int.Parse(strings[2]);
                    _entries.Add(new CreateModifyEntry(frame, (RPeakLabel)labelNum));
                    break;
                case RPeaksModifyEntryType.DELETE:
                    frame = long.Parse(strings[1]);
                    _entries.Add(new DeleteModifyEntry(frame));
                    break;
                case RPeaksModifyEntryType.UPDATE_LABEL:
                    frame = long.Parse(strings[1]);
                    oldLabelNum = int.Parse(strings[2]);
                    newLabelNum = int.Parse(strings[3]);
                    _entries.Add(new UpdateLabelModifyEntry(frame, (RPeakLabel)oldLabelNum, (RPeakLabel)newLabelNum));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public async Task AddRPeakPointAsync(long time, RPeakLabel label)
    {
        var frame = TimeToFrame(time);
        _entries.Add(new CreateModifyEntry(frame, label));
        await using var sw = File.AppendText(_modificationPath);
        await sw.WriteLineAsync($"c;{frame};{(int)label}");
    }

    public async Task DeleteRPeakPointAsync(long time)
    {
        var frame = TimeToFrame(time);
        _entries.Add(new DeleteModifyEntry(frame));
        await using var sw = File.AppendText(_modificationPath);
        await sw.WriteLineAsync($"d;{frame}");
    }

    public async Task UpdateRPeakPointLabel(long time, RPeakLabel oldLabel, RPeakLabel newLabel)
    {
        var frame = TimeToFrame(time);
        _entries.Add(new UpdateLabelModifyEntry(frame, oldLabel, newLabel));
        await using var sw = File.AppendText(_modificationPath);
        await sw.WriteLineAsync($"ul;{frame};{(int)oldLabel};{(int)newLabel}");
    }

    private List<RPeakUnit> GetAfterModifyData(long beginTime, long lastTime, List<RPeakUnit> originalData)
    {
        var endTime = beginTime + lastTime;
        var isBetweenTime = (long time) => time > beginTime && time < endTime;
        long time;
        RPeakLabel label, newLabel;
        foreach (var entry in _entries)
        {
            switch (entry.Type)
            {
                case RPeaksModifyEntryType.CREATE:
                    var createModifyEntry = (CreateModifyEntry)entry;
                    time = FrameToTime(createModifyEntry.Frame);
                    label = createModifyEntry.Label;
                    if (isBetweenTime(time))
                    {
                        var index = originalData.FindIndex(value => value.Time > time);
                        if (index != -1)
                            originalData.Insert(index, new RPeakUnit(label, time));
                        else originalData.Add(new RPeakUnit(label, time));
                    }

                    break;
                case RPeaksModifyEntryType.DELETE:
                    var deleteModifyEntry = (DeleteModifyEntry)entry;
                    originalData.RemoveAll(value => value.Time == FrameToTime(deleteModifyEntry.Frame));
                    break;

                case RPeaksModifyEntryType.UPDATE_LABEL:
                    var updateLabelModifyEntry = (UpdateLabelModifyEntry)entry;
                    newLabel = updateLabelModifyEntry.NewLabel;
                    foreach (var rPeakUnitValue in originalData.Where(rPeakUnitValue =>
                                 rPeakUnitValue.Time == FrameToTime(updateLabelModifyEntry.Frame)))
                        rPeakUnitValue.Label = newLabel;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return originalData;
    }

    public void Dispose()
    {
        _fileStreamPool.Foreach(fs => { fs.Dispose(); });
    }
}