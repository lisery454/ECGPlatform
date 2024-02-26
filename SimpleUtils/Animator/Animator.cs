using System.Numerics;

namespace SimpleUtils;

public class Animator : IDisposable
{
    private readonly Func<double> _getFunc;
    private readonly Action<double> _setFunc;

    private double Current => _getFunc();
    private double Target { get; set; }

    private readonly CancellationTokenSource _cts;
    private readonly IAnimationType _animationType;

    private readonly TimeSpan _timeInterval;

    public Animator(Func<double> getFunc, Action<double> setFunc, TimeSpan timeInterval, IAnimationType animationType)
    {
        _getFunc = getFunc;
        _setFunc = setFunc;
        Target = getFunc();
        _timeInterval = timeInterval;
        _animationType = animationType;
        _cts = new CancellationTokenSource();
        Animate(_cts.Token).Await();
    }

    private async Task Animate(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var nextState = _animationType.GetNextState(Current, Target, _timeInterval);
            if (nextState != null)
                _setFunc(nextState.Value);
            await Task.Delay(_timeInterval, cancellationToken);
        }
    }

    public void ChangeTarget(double target)
    {
        Target = target;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}