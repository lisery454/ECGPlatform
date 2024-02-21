namespace SimpleUtils;

public static class CtsUtils
{
    public static CancellationTokenSource Refresh(ref CancellationTokenSource? source)
    {
        if (source != null)
        {
            source.Cancel();
            source.Dispose();
            source = null;
        }

        source = new CancellationTokenSource();
        return source;
    }
}