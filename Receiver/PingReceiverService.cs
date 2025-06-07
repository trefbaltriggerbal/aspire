using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class PingReceiverService : BackgroundService
{
    private readonly ILogger<PingReceiverService> _logger;

    public PingReceiverService(ILogger<PingReceiverService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        _logger.LogDebug("Receiver listening on http://localhost:5000/");

        while (!stoppingToken.IsCancellationRequested)
        {
            var context = await listener.GetContextAsync();
            if (context.Request.Url?.AbsolutePath == "/ping")
            {
                var responseString = "Pong";
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length, stoppingToken);
                context.Response.Close();
                _logger.LogDebug("Responded with {Response}", responseString);
            }
        }
    }
}
