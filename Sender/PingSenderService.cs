using System.Net.Http;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class PingSenderService : BackgroundService
{
    private readonly ILogger<PingSenderService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly ActivitySource ActivitySource = new("Projects.Sender");

    public PingSenderService(ILogger<PingSenderService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("SendPing");
        var client = _httpClientFactory.CreateClient();
        _logger.LogInformation("Sending request to receiver...");
        var response = await client.GetStringAsync("http://localhost:5000/ping", stoppingToken);
        _logger.LogInformation("Received response {Response}", response);
    }
}
