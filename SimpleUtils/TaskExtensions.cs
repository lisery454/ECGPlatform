namespace SimpleUtils;

public static class TaskExtensions
{
    public static async void Await<T>(this Task<T> task,
        Action<T>? onCompleted = null,
        Action<Exception>? onError = null)
    {
        try
        {
            var t = await task;
            onCompleted?.Invoke(t);
        }
        catch (Exception e)
        {
            onError?.Invoke(e);
        }
    }

    public static async void Await(this Task task,
        Action? onCompleted = null,
        Action<Exception>? onError = null)
    {
        try
        {
            await task;
            onCompleted?.Invoke();
        }
        catch (Exception e)
        {
            onError?.Invoke(e);
        }
    }
}