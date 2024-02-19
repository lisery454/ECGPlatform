namespace ECGFileService;

public class PointData
{
    public readonly long time; // milli seconds
    public readonly float value;

    public PointData(long time, float value)
    {
        this.time = time;   
        this.value = value;
    }
}