namespace ECGPlatform;

public partial class ShowECGWindowViewModel
{
    private readonly Lazy<ObjectPool<HighlightPoint>> _rPeakPointPool;
    private Canvas HighlightPointCanvas => ((ShowECGWindow)BindingWindow!).HighlightPointCanvas;

    private CartesianChart CartesianChart => ((ShowECGWindow)BindingWindow!).CartesianChart;

    private void HideAllRPeakPoints()
    {
        foreach (HighlightPoint child in HighlightPointCanvas.Children)
            _rPeakPointPool.Value.Release(child, point =>
            {
                point.Visibility = Visibility.Hidden;
                point.ClearBinding(HighlightPoint.ClickedCommandProperty);
                point.IsSelected = false;
            });
    }
}