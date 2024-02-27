namespace ECGFileService;

public class HighlightPointData : IEquatable<HighlightPointData>
{
    public long Time { get; }
    public List<float> Value { get; }
    public int? Id { get; }

    public string Letter => Id == null ? "?" : RPeakUnit.FromIdToLetter(Id.Value);
    public string? Label => Id == null ? null : RPeakUnit.FromIdToLabel(Id.Value);
    public PointType PointType => Id == null ? PointType.SIMPLE_POINT : PointType.R_PEAKS_POINT;

    public HighlightPointData(long time, List<float> value, int id)
    {
        Time = time;
        Value = value;
        Id = id;
    }

    public HighlightPointData(long time, List<float> value)
    {
        Time = time;
        Value = value;
        Id = null;
    }


    public static bool operator ==(HighlightPointData lhs, HighlightPointData? rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(HighlightPointData lhs, HighlightPointData? rhs) => !(lhs == rhs);

    public bool Equals(HighlightPointData? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Time == other.Time && Value.Equals(other.Value) && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((HighlightPointData)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Time, Value, Id);
    }
}