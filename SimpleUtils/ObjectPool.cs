namespace SimpleUtils;

public class ObjectPool<T> where T : notnull
{
    private readonly Func<T> _factoryFunction;

    private readonly Dictionary<T, bool> _pool;

    public int Size => _pool.Count;

    public bool IsAvailable(T obj)
    {
        if (_pool.TryGetValue(obj, out var isAvailable))
        {
            return isAvailable;
        }

        throw new Exception("this obj is not in pool.");
    }

    private void SetIsAvailable(T obj, bool isAvailable)
    {
        if (_pool.ContainsKey(obj))
        {
            _pool[obj] = isAvailable;
        }
        else
            throw new Exception("this obj is not in pool.");
    }

    public ObjectPool(Func<T> factoryFunction, int originalSize)
    {
        _factoryFunction = factoryFunction;
        _pool = new Dictionary<T, bool>();
        for (var i = 0; i < originalSize; i++)
        {
            _pool.Add(_factoryFunction.Invoke(), true);
        }
    }

    public T Get(Action<T>? onGet = null)
    {
        foreach (var (obj, isAvail) in _pool)
        {
            if (isAvail)
            {
                SetIsAvailable(obj, false);
                onGet?.Invoke(obj);
                return obj;
            }
        }

        var newObj = _factoryFunction.Invoke();
        _pool.Add(newObj, false);
        onGet?.Invoke(newObj);
        return newObj;
    }

    public void Release(T obj, Action<T>? onRelease = null)
    {
        if (_pool.ContainsKey(obj))
        {
            onRelease?.Invoke(obj);
            SetIsAvailable(obj, true);
        }
        else
            throw new Exception("this obj is not in pool.");
    }

    public void Foreach(Func<T, bool> condition, Action<T> action)
    {
        foreach (var (obj, _) in _pool)
        {
            if (condition.Invoke(obj))
            {
                action.Invoke(obj);
            }
        }
    }

    public void Foreach(Action<T> action)
    {
        foreach (var (obj, _) in _pool)
        {
            action.Invoke(obj);
        }
    }

    public void ForeachNotAvail(Action<T> action)
    {
        foreach (var (obj, isAvail) in _pool)
        {
            if (!isAvail)
            {
                action.Invoke(obj);
            }
        }
    }
}