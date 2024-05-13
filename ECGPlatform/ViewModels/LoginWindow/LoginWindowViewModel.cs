namespace ECGPlatform;

public partial class LoginWindowViewModel : WindowBaseViewModel
{
    [ObservableProperty] private string _username = "patient2";
    [ObservableProperty] private string _password = "654321";
    [ObservableProperty] private string _infoStr = string.Empty;

    private readonly IHttpManager _httpManager;

    public LoginWindowViewModel(IHttpManager httpManager)
    {
        _httpManager = httpManager;
    }

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            InfoStr = "登录中...";
            await _httpManager.AccountLogin(Username, Password);

            BindingWindow!.Closing += (_, _) =>
            {
                var mainWindow = App.Current.Services.GetService<MainWindow>()!;
                mainWindow.Show();
            };
            BindingWindow?.Close();
        }
        catch (Exception)
        {
            InfoStr = "登录失败";
        }
    }
}