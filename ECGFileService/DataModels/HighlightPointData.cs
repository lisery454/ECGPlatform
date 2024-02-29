namespace ECGFileService;

public class HighlightPointData : IEquatable<HighlightPointData>
{
    public long Time { get; }
    public List<float> Values { get; }
    public int? Id { get; }

    public string Letter => Id == null ? "?" : RPeakUnit.FromIdToLetter(Id.Value);
    public string? Label => Id == null ? null : RPeakUnit.FromIdToLabel(Id.Value);
    public PointType PointType => Id == null ? PointType.SIMPLE_POINT : PointType.R_PEAKS_POINT;

    public HighlightPointData(long time, List<float> values, int id)
    {
        Time = time;
        Values = values;
        Id = id;
    }

    public HighlightPointData(long time, List<float> values)
    {
        Time = time;
        Values = values;
        Id = null;
    }


    public override string ToString()
    {
        return $"time {Time}; value: {Values}; id: {Id}";
    }

    public static bool operator ==(HighlightPointData? lhs, HighlightPointData? rhs)
    {
        return lhs?.Equals(rhs) ?? ReferenceEquals(null, rhs);
    }

    public static bool operator !=(HighlightPointData? lhs, HighlightPointData? rhs) => !(lhs == rhs);

    public bool Equals(HighlightPointData? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Time == other.Time && Values.Equals(other.Values) && Id == other.Id;
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
        return HashCode.Combine(Time, Values, Id);
    }
}