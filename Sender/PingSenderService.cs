using System.Net.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class PingSenderService : BackgroundService
{
    private readonly ILogger<PingSenderService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public PingSenderService(ILogger<PingSenderService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _httpClientFactory.CreateClient();
        _logger.LogDebug("Sending request to receiver...");
        var response = await client.GetStringAsync("http://localhost:5000/ping", stoppingToken);
        _logger.LogDebug("Received response {Response}", response);
    }
}
