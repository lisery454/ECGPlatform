namespace ECGPlatform;

public class HttpManager : IHttpManager
{
    private static string Address => "http://localhost:8080";

    private ProgramConstants _programConstants;

    private string? _token;

    private HttpClient _httpClient = new() { BaseAddress = new Uri(Address) };

    public HttpManager(ProgramConstants programConstants)
    {
        _programConstants = programConstants;
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
    }

    private static async Task<dynamic> GetResultFromHttpMessage(HttpResponseMessage httpResponseMessage)
    {
        var result = await httpResponseMessage.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(result) ?? throw new InvalidOperationException();
    }

    private static ByteArrayContent GetJsonContent(object obj)
    {
        var serializeObject = JsonConvert.SerializeObject(obj);
        var buffer = Encoding.UTF8.GetBytes(serializeObject);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return byteContent;
    }

    public async Task AccountLogin(string username, string password)
    {
        var byteContent = GetJsonContent(new { username, password });
        var httpResponseMessage = await _httpClient.PostAsync("account/login", byteContent);
        var result = await GetResultFromHttpMessage(httpResponseMessage);
        var success = (bool)result.success;
        if (!success)
        {
            throw new Exception(result.message);
        }

        _token = (string)result.data.token;
        _httpClient.DefaultRequestHeaders.Add("token", _token);
    }

    public async Task<List<LabelTask>> TaskList()
    {
        var httpResponseMessage = await _httpClient.GetAsync("task/list");
        var result = await GetResultFromHttpMessage(httpResponseMessage);
        var success = (bool)result.success;
        if (!success)
        {
            throw new Exception(result.message);
        }


        var taskResult = new List<LabelTask>();
        foreach (var task in result.data)
        {
            var id = (int)task.taskId;
            var title = (string)task.title;
            var description = (string)task.description;
            var fragmentIds = JsonConvert.DeserializeObject<List<int>>((string)task.fragmentIds);
            taskResult.Add(new LabelTask(id, title, fragmentIds!, description));
        }
        
        return taskResult;
    }

    public async Task GetFragment(int taskId, int fragmentId)
    {
        // get fragment info
        var httpResponseMessage = await _httpClient.GetAsync($"fragment/id?fragmentId={fragmentId}");
        var result = await GetResultFromHttpMessage(httpResponseMessage);
        var success = (bool)result.success;
        if (!success)
        {
            throw new Exception(result.message);
        }

        var rate = (int)result.data.rate;
        var content = JsonConvert.DeserializeObject<List<dynamic>>((string)result.data.content)!;
        var data = ((JArray)content[0].data).ToObject<List<int>>()!;
        var rawDataId = (string)result.data.rawDataId;
        var begin = (long)result.data.offset;
        var length = (long)result.data.length;
        var end = begin + length;

        // get label info
        var httpResponseMessage2 =
            await _httpClient.GetAsync($"label/list?rawDataId={rawDataId}&start={begin}&end={end}");
        var result2 = await GetResultFromHttpMessage(httpResponseMessage2);
        if (!success)
        {
            throw new Exception(result.message);
        }

        var rPeakUnits = new List<RPeakUnit>();

        foreach (var subData in result2.data)
        {
            var position = (long)subData.position - begin;
            var labelStr = (string)subData.content;

            rPeakUnits.Add(new RPeakUnit(RPeakLabelConvert.StrToLabel(labelStr), position * 1000 / rate));
        }

        // write into files
        var directoryPath = Path.Combine(_programConstants.DefaultRemoteDataDirectoryPath, $"{taskId}");
        await ECGFileWriter.Write(directoryPath, fragmentId, data, rate, rPeakUnits);
    }
}