namespace ECGFileService;

public class HighlightPointData : IEquatable<HighlightPointData>
{
    public long Time { get; }
    public List<float> Values { get; }
    public RPeakLabel? Label { get; }

    public string Letter => Label == null ? "?" : RPeakUnit.FromLabelToLetter(Label.Value);

    public PointType PointType => Label == null ? PointType.SIMPLE_POINT : PointType.R_PEAKS_POINT;

    public HighlightPointData(long time, List<float> values, RPeakLabel label)
    {
        Time = time;
        Values = values;
        Label = label;
    }

    public HighlightPointData(HighlightPointData highlightPointData)
    {
        Time = highlightPointData.Time;
        Values = new List<float>(highlightPointData.Values);
        Label = highlightPointData.Label;
    }

    public HighlightPointData(long time, List<float> values)
    {
        Time = time;
        Values = values;
        Label = null;
    }


    public override string ToString()
    {
        return $"time {Time}; value: {Values}; label: {Label}";
    }

    public static bool operator ==(HighlightPointData? lhs, HighlightPointData? rhs)
    {
        if (ReferenceEquals(null, lhs))
            return ReferenceEquals(null, rhs);

        if (ReferenceEquals(null, rhs)) return false;
        
        return lhs.Equals(rhs);
    }

    public static bool operator !=(HighlightPointData? lhs, HighlightPointData? rhs) => !(lhs == rhs);

    public bool Equals(HighlightPointData? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Time == other.Time && Values.All(other.Values.Contains) && Values.Count == other.Values.Count &&
               Label == other.Label;
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
        return HashCode.Combine(Time, Values, Label);
    }
}