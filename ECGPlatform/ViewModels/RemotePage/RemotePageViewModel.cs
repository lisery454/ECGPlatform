namespace ECGPlatform;

public partial class RemotePageViewModel : ObservableObject
{
    [RelayCommand]
    private void Init()
    {
        // 加载登录信息
    }

    [RelayCommand]
    private void OpenLoginWindow()
    {
        var loginWindow = App.Current.Services.GetService<LoginWindow>()!;
        var showDialog = loginWindow.ShowDialog();

        if (showDialog is true)
        {
            Init(); // 重新刷新界面
        }
    }
}
