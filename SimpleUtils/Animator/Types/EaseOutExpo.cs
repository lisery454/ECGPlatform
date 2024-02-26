namespace SimpleUtils;

public class EaseOutExpo : IAnimationType
{
    public double MoveThreshold { get; }
    public double StopThreshold { get; }

    public float Coefficient { get; }

    public EaseOutExpo(double moveThreshold, double stopThreshold, float coefficient)
    {
        MoveThreshold = moveThreshold;
        StopThreshold = stopThreshold;
        Coefficient = coefficient;
    }


    public double? GetNextState(double current, double target, TimeSpan timeInterval)
    {
        var distance = Math.Abs(current - target);
        // var sign = Math.Sign(current - target);

        if (distance < MoveThreshold) return null;
        if (distance < StopThreshold) return target;
        return current + (target - current) * Coefficient;
    }
}