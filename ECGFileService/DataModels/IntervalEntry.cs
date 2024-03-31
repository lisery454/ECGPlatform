namespace ECGFileService;

public class IntervalEntry
{
    public long startTime;
    public long endTime;

    public MarkIntervalLabel markIntervalLabel;

    public IntervalEntry(long startTime, long endTime, MarkIntervalLabel markIntervalLabel)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.markIntervalLabel = markIntervalLabel;
    }
}