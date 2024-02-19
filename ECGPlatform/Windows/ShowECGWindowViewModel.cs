namespace ECGPlatform;

public partial class ShowECGWindowViewModel : WindowBaseViewModel
{
    private readonly ECGFileManager _ecgFileManager;

    private ECGIndex? _ecgIndex;

    public ECGIndex? EcgIndex
    {
        get => _ecgIndex;
        set
        {
            _ecgIndex = value;
            _ecgFileManager.Load(_ecgIndex!);
        }
    }

    public ShowECGWindowViewModel()
    {
        _ecgFileManager = new ECGFileManager();
        Closed += () => _ecgFileManager.Dispose();
    }
}