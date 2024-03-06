namespace ECGPlatform;

public partial class TimeLabel : UserControl
{
    public TimeLabel()
    {
        InitializeComponent();
    }

    #region TimeInterval

    public static readonly DependencyProperty TimeIntervalProperty =
        DependencyProperty.Register(nameof(TimeInterval), typeof(long),
            typeof(TimeLabel),
            new FrameworkPropertyMetadata(0L, OnTimeIntervalChanged) { BindsTwoWayByDefault = true });

    public long TimeInterval
    {
        get => (long)GetValue(TimeIntervalProperty);
        set => SetValue(TimeIntervalProperty, value);
    }

    private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var timeLabel = (d as TimeLabel)!;
        timeLabel.HeartRate = (long)(60000f / timeLabel.TimeInterval);
    }

    #endregion

    private long _heartRate;

    public long HeartRate
    {
        get => _heartRate;
        set => SetField(ref _heartRate, value);
    }
}

public partial class TimeLabel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}