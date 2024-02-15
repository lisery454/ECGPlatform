namespace ECG_Platform;

public abstract class BaseAttachedProperty<TParent, TProperty>
    where TParent : BaseAttachedProperty<TParent, TProperty>, new()
{
    public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, e) => { };

    public static TParent Instance { get; } = new();

    public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value",
        typeof(TProperty), typeof(BaseAttachedProperty<TParent, TProperty>),
        new UIPropertyMetadata((d, e) =>
        {
            Instance.OnValueChanged(d, e);
            Instance.ValueChanged(d, e);
        }));

    public static TProperty GetValue(DependencyObject d) => (TProperty)d.GetValue(ValueProperty);

    public static void SetValue(DependencyObject d, TProperty value) => d.SetValue(ValueProperty, value);

    public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
    }
}