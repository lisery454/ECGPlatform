namespace SimpleUtils;

public interface IAnimationType 
{
    public double MoveThreshold { get; }
    public double StopThreshold { get; }
    
    double? GetNextState(double current, double target, TimeSpan timeInterval);
}