namespace SimpleUtils;

public static class ObjectExtensions
{
    public static void Debug(this object? self)
    {
        System.Diagnostics.Debug.WriteLine(self == null ? "null" : $"{self.GetType().Name}: {self}");
    }
}