namespace ECGFileService;

public struct RIntervalData
{
    public long Time { get; } // milli seconds
    public long Interval { get; } // milli seconds

    public RIntervalData(long time, long interval)
    {
        Time = time;
        Interval = interval;
    }
}