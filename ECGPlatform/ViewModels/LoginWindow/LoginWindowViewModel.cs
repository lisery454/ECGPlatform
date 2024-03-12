using System.Net.Http;

namespace ECGPlatform;

public partial class LoginWindowViewModel : WindowBaseViewModel
{
    [RelayCommand]
    private void Cancel()
    {
        BindingWindow!.DialogResult = false;
        BindingWindow!.Close();
    }

    [RelayCommand]
    private async Task Login()
    {
        // TODO 执行一些登录操作，如果登录成功，保存登录状态
        var httpClient = new HttpClient { BaseAddress = new Uri("https://catfact.ninja") };
        httpClient.Timeout = TimeSpan.FromSeconds(5);
        
        var httpResponseMessage = await httpClient.GetAsync("fact");
        httpResponseMessage.Debug();
        var result = await httpResponseMessage.Content.ReadAsStringAsync();
        result.Debug();
        
        BindingWindow!.DialogResult = true;
        BindingWindow!.Close();
    }
}