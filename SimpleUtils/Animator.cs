using System.Numerics;

namespace SimpleUtils;

public delegate void Move<in T>(T current, T target, bool isTargetChanged) where T : INumber<T>;

public class Animator<T> : IDisposable where T : INumber<T>
{
    private readonly Move<T> _moveFunc;
    private readonly Func<T> _getFunc;

    private T Current => _getFunc();
    private T Target { get; set; }

    private readonly CancellationTokenSource _cts;
    private readonly TimeSpan _timeInterval;
    private bool _isTargetChanged;

    public Animator(Func<T> getFunc, TimeSpan timeInterval, Move<T> moveFunc)
    {
        _getFunc = getFunc;
        _moveFunc = moveFunc;
        Target = getFunc();
        _timeInterval = timeInterval;
        _isTargetChanged = false;
        _cts = new CancellationTokenSource();
        Animate(_cts.Token).Await();
    }

    private async Task Animate(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _moveFunc(Current, Target, _isTargetChanged);
            _isTargetChanged = false;
            await Task.Delay(_timeInterval, cancellationToken);
        }
    }

    public void ChangeTarget(T target)
    {
        Target = target;
        _isTargetChanged = true;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}