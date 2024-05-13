namespace ECGPlatform;

public interface IHttpManager
{
    public Task AccountLogin(string username, string password);

    public Task<List<LabelTask>> TaskList();

    public Task GetFragment(int taskId, int fragmentId);
}