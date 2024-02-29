namespace ECGFileService;

public partial class RPeakUnit
{
    public long Time { get; set; }
    public int Id { get; set; }

    public RPeakLabel Label => FromIdToLabel(Id);

    public RPeakUnit(int id, long time)
    {
        Id = id;
        Time = time;
    }
}

public partial class RPeakUnit
{
    private static readonly Dictionary<RPeakLabel, int> LabelMap = new()
    {
        { RPeakLabel.NONE, 0 },
        { RPeakLabel.SINUS_RHYTHM, 1 },
        { RPeakLabel.VENTRICULAR_PREEXCITATION, 2 },
        { RPeakLabel.PREMATURE_ATRIAL_CONTRACTIONS, 3 },
        { RPeakLabel.PREMATURE_VENTRICULAR_CONTRACTIONS, 4 },
        { RPeakLabel.ATRIAL_FIBRILLATION, 5 },
        { RPeakLabel.ATRIAL_FLUTTER, 6 },
        { RPeakLabel.VENTRICULAR_FLUTTER_VENTRICULAR_FIBRILLATION, 7 },
        { RPeakLabel.ATRIOVENTRICULAR_BLOCK, 8 },
        { RPeakLabel.NOISE, 9 },
        { RPeakLabel.PAROXYSMAL_SUPRAVENTRICULAR_TACHYCARDIA, 10 },
    };

    public static readonly List<RPeakLabel> AllLabels = new();

    static RPeakUnit()
    {
        foreach (var (_, value) in LabelMap2.Value)
        {
            AllLabels.Add(value);
        }
    }

    private static readonly Lazy<Dictionary<int, RPeakLabel>> LabelMap2 = new(() =>
    {
        var map = new Dictionary<int, RPeakLabel>();
        foreach (var (label, id) in LabelMap)
        {
            map.TryAdd(id, label);
        }

        return map;
    });

    public static int FromLabelToId(RPeakLabel label)
    {
        return LabelMap[label];
    }

    public static RPeakLabel FromIdToLabel(int id)
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