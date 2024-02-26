namespace ECGFileService;

public partial class RPeakUnit
{
    public long Time { get; set; }
    public int Id { get; set; }

    public string Label => FromIdToLabel(Id);

    public RPeakUnit(int id, long time)
    {
        Id = id;
        Time = time;
    }
}

public partial class RPeakUnit
{
    private static readonly Dictionary<string, int> LabelMap = new()
    {
        { "无标签", 0 },
        { "窦性心律", 1 },
        { "心室预激", 2 },
        { "房性早搏", 3 },
        { "室性早搏", 4 },
        { "心房颤动", 5 },
        { "心房扑动", 6 },
        { "室扑室颤", 7 },
        { "房室传导阻滞", 8 },
        { "噪声", 9 },
        { "噪音", 9 },
        { "阵发性室上性心动过速", 10 },
    };

    public static readonly List<string> AllLabels = new();

    static RPeakUnit()
    {
        foreach (var (_, value) in LabelMap2.Value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                AllLabels.Add(value);
            }
        }
    }

    private static readonly Lazy<Dictionary<int, string>> LabelMap2 = new(() =>
    {
        var map = new Dictionary<int, string>();
        foreach (var (label, id) in LabelMap)
        {
            map.TryAdd(id, label);
        }

        return map;
    });

    public static int FromLabelToId(string label)
    {
        return LabelMap[label];
    }

    public static string FromIdToLabel(int id)
    {
        return LabelMap2.Value[id];
    }


    // TODO 
    static string a = "abcdefghijklmnopqrstuvwxyz";

    public static string FromIdToLetter(int id)
    {
        return a[id].ToString();
    }
}