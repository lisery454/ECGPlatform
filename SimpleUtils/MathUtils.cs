namespace SimpleUtils;

public static class MathUtils
{
    public static long Clamp(long value, long min, long max)
    {
        if (min > max)
        {
            throw new ArgumentException("最大值小于最大值");
        }

        if (value < min) return min;
        if (value > max) return max;

        return value;
    }
}