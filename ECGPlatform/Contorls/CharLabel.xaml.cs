namespace ECGPlatform;

public partial class CharLabel : UserControl
{
    public CharLabel()
    {
        InitializeComponent();
    }

    #region Char

    public static readonly DependencyProperty CharProperty =
        DependencyProperty.Register(nameof(Char), typeof(string),
            typeof(CharLabel),
            new FrameworkPropertyMetadata("N") { BindsTwoWayByDefault = true });

    public string Char
    {
        get => (string)GetValue(CharProperty);
        set => SetValue(CharProperty, value);
    }

    #endregion
}