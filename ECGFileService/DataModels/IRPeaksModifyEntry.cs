namespace ECGFileService;

public interface IRPeaksModifyEntry
{
    public RPeaksModifyEntryType Type { get; }

    public static RPeaksModifyEntryType StrToType(string str)
    {
        return str.ToLower() switch
        {
            "c" => RPeaksModifyEntryType.CREATE,
            "d" => RPeaksModifyEntryType.DELETE,
            "ul" => RPeaksModifyEntryType.UPDATE_LABEL,
            _ => throw new Exception("Unknown RPeaksModifyEntryType")
        };
    }
}

public class CreateModifyEntry : IRPeaksModifyEntry
{
    public RPeaksModifyEntryType Type => RPeaksModifyEntryType.CREATE;
    public long Frame { get; }

    public RPeakLabel Label { get; }

    public CreateModifyEntry(long frame, RPeakLabel label)
    {
        Frame = frame;
        Label = label;
    }
}

public class DeleteModifyEntry : IRPeaksModifyEntry
{
    public RPeaksModifyEntryType Type => RPeaksModifyEntryType.DELETE;
    public long Frame { get; }

    public DeleteModifyEntry(long frame)
    {
        Frame = frame;
    }
}

public class UpdateLabelModifyEntry : IRPeaksModifyEntry
{
    public RPeaksModifyEntryType Type { get; } = RPeaksModifyEntryType.UPDATE_LABEL;

    public long Frame { get; }

    public RPeakLabel OldLabel { get; }
    public RPeakLabel NewLabel { get; }

    public UpdateLabelModifyEntry(long frame, RPeakLabel oldLabel, RPeakLabel newLabel)
    {
        Frame = frame;
        OldLabel = oldLabel;
        NewLabel = newLabel;
    }
}

public enum RPeaksModifyEntryType
{
    CREATE,
    DELETE,
    UPDATE_LABEL,
}