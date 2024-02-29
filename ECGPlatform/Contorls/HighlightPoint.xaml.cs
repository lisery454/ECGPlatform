namespace ECGPlatform;

public sealed partial class HighlightPoint
{
    public HighlightPoint()
    {
        InitializeComponent();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Handled) return;
        ClickedCommand?.Execute(HighlightPointData);
        e.Handled = true;
    }

    private HighlightPointData? _highlightPointData;

    public HighlightPointData? HighlightPointData
    {
        get => _highlightPointData;
        set => SetField(ref _highlightPointData, value);
    }

    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }

    #region ClickedCommand

    public static readonly DependencyProperty ClickedCommandProperty =
        DependencyProperty.Register(nameof(ClickedCommand), typeof(IRelayCommand<HighlightPointData>),
            typeof(HighlightPoint),
            new FrameworkPropertyMetadata(null));

    public IRelayCommand<HighlightPointData>? ClickedCommand
    {
        get => (IRelayCommand<HighlightPointData>?)GetValue(ClickedCommandProperty);
        set => SetValue(ClickedCommandProperty, value);
    }

    #endregion
}

public sealed partial class HighlightPoint : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}