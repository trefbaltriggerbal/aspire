namespace UiFlowRecorder;

// Re-added support classes

internal static class Config
{
    public static readonly Uri BaseUrl = new("http://localhost:5228");
    public const string UploadEndpoint = "https://0x0.st";
    public const string PersistFile = "UploadMap2.json";
    public const string UserAgent = "curl/8.5.0";
    public static readonly HttpClient Http = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        return client;
    }
}
