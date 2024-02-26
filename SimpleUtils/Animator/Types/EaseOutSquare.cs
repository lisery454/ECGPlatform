namespace SimpleUtils;

public class EaseOutSquare : IAnimationType
{
    public double MoveThreshold { get; }
    public double StopThreshold { get; }

    private double _oldCurrent;
    private double _oldTarget;

    private TimeSpan UseTime { get; }
    private double t;

    public EaseOutSquare(double moveThreshold, double stopThreshold, TimeSpan useTime)
    {
        MoveThreshold = moveThreshold;
        StopThreshold = stopThreshold;
        UseTime = useTime;
    }


    public double? GetNextState(double current, double target, TimeSpan timeInterval)
    {
        if (Math.Abs(_oldTarget - target) > 0.05d)
        {
            _oldTarget = target;
            _oldCurrent = current;
            t = 0;
        }

        var distance = Math.Abs(current - target);

        if (distance < MoveThreshold) return null;
        if (distance < StopThreshold) return target;

        var I = timeInterval.TotalMilliseconds;
        var T = UseTime.TotalMilliseconds;
        var index = (T - t) / T;

        var res = (_oldCurrent - _oldTarget) * index * index + _oldTarget;
        t += I;

        if (t > T) return target;
        
        return res;
    }
}