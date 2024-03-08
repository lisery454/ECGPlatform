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
        set
        {
            if (SetField(ref _highlightPointData, value))
            {
                if (_highlightPointData == null)
                {
                    LabelText = string.Empty;
                    IsShowLabel = false;
                }
                else if (_highlightPointData.Label == null)
                {
                    LabelText = string.Empty;
                    IsShowLabel = false;
                }
                else
                {
                    LabelText = RPeakLabelToStrConverter.Instance.Convert(_highlightPointData.Label.Value);
                    IsShowLabel = true;
                }
            }
        }
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

    public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string),
            typeof(HighlightPoint),
            new FrameworkPropertyMetadata(string.Empty) { BindsTwoWayByDefault = true });

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    private void LabelTextBlock_OnLayoutUpdated(object? sender, EventArgs e)
    {
        LabelTextBlock.Margin =
            new Thickness(-LabelTextBlock.ActualWidth / 2, -LabelTextBlock.ActualHeight / 2, 0, 0);
    }

    public bool IsShowLabel
    {
        get => (bool)GetValue(IsShowLabelProperty);
        set => SetValue(IsShowLabelProperty, value);
    }

    public static readonly DependencyProperty IsShowLabelProperty =
        DependencyProperty.Register(nameof(IsShowLabel), typeof(bool),
            typeof(HighlightPoint),
            new FrameworkPropertyMetadata(false) { BindsTwoWayByDefault = true });

    public void Reset()
    {
        ScaleTransform.ScaleX = (double)FindResource("UnSelectedSize");
        ScaleTransform.ScaleY = (double)FindResource("UnSelectedSize");
        Border.Opacity = (double)FindResource("UnSelectedOpacity");
    }
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