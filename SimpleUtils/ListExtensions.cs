namespace SimpleUtils;

public static class ListExtensions
{
    public static string ToFormat<T>(this List<T> list, Func<T, T>? onFormat = null) where T : notnull
    {
        var res = string.Empty;
        res += "[";
        res = onFormat == null
            ? list.Aggregate(res, (current, element) => current + (element + ", "))
            : list.Aggregate(res, (current, element) => current + (onFormat(element) + ", "));
        var lastIndexOf = res.LastIndexOf(", ", StringComparison.Ordinal);
        res = res.Remove(lastIndexOf);
        res += "]";
        return res;
    }
}