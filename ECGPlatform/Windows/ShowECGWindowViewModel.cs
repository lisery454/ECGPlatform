namespace ECGPlatform;

public partial class ShowECGWindowViewModel : WindowBaseViewModel
{
    private readonly ECGFileManager _ecgFileManager;

    private ECGIndex? _ecgIndex;

    public event Action Closed;

    public ECGIndex? EcgIndex
    {
        get => _ecgIndex;
        set
        {
            _ecgIndex = value;
            _ecgFileManager.Load(_ecgIndex!);
        }
    }

    public ShowECGWindowViewModel(ECGFileManager ecgFileManager)
    {
        _ecgFileManager = ecgFileManager;
        Closed += () => _ecgFileManager.Dispose();
    }

    [RelayCommand]
    private void OnClosed()
    {
        Closed();
    }
}