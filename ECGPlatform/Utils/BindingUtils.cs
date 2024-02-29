namespace ECGPlatform;

public static class BindingUtils
{
    public record struct Binder(DependencyObject dependencyObject, DependencyProperty dp)
    {
        public readonly DependencyObject dependencyObject = dependencyObject;
        public readonly DependencyProperty dp = dp;
    }

    public static void ClearBinding(this DependencyObject dependencyObject, DependencyProperty dp)
    {
        BindingOperations.ClearBinding(dependencyObject, dp);
    }

    public static Binder Binding(this DependencyObject dependencyObject, DependencyProperty dp)
    {
        return new Binder(dependencyObject, dp);
    }

    public static void To(this Binder binder, object source, PropertyPath propertyPath,
        BindingMode bindingMode = BindingMode.OneWay,
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.PropertyChanged)
    {
        var binding = new Binding
        {
            Source = source,
            Path = propertyPath,
            Mode = bindingMode,
            UpdateSourceTrigger = updateSourceTrigger
        };

        BindingOperations.SetBinding(binder.dependencyObject, binder.dp, binding);
    }
}