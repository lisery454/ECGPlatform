namespace SimpleUtils;

public class TimeFormatter
{
    public static string MircoSecondsToString(long ms)
    {
        var s = ms / 1000;
        ms %= 1000;
        var m = s / 60;
        s %= 60;
        var h = m / 60;
        m %= 60;
        return $"{h}:{m:D2}:{s:D2}.{ms:D3}";
    }

    public static bool TryParseTimeMsFromStr(string timeStr, out int milliSeconds)
    {
        milliSeconds = 0;
        var split = timeStr.Split(':');

        if (split.Length != 3) return false;
        
        var hStr = split[0];
        var mStr = split[1];
        var split2 = split[2].Split('.');
        
        if (split2.Length != 2) return false;
        
        var sStr = split2[0];
        var msStr = split2[1];

        if (!int.TryParse(hStr, out var h) ||
            !int.TryParse(mStr, out var m) ||
            !int.TryParse(sStr, out var s) ||
            !int.TryParse(msStr, out var ms)) return false;
        
        if (m is < 0 or > 59 || s is < 0 or > 59 || ms is < 0 or > 999) return false;
        
        var allTime = ms + 1000 * (s + 60 * (m + 60 * h));
        
        milliSeconds = allTime;
        
        return true;
    }
}