using System.Net;
using System.Diagnostics;
using OpenTelemetry.Context.Propagation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class PingReceiverService : BackgroundService
{
    private readonly ILogger<PingReceiverService> _logger;
    private static readonly ActivitySource ActivitySource = new("Projects.Receiver");
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

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
                var parentContext = Propagator.Extract(default, context.Request.Headers,
                    static (headers, name) => headers.GetValues(name) ?? Array.Empty<string>());

                using var activity = ActivitySource.StartActivity(
                    "HandlePing", ActivityKind.Server, parentContext.ActivityContext);
                var responseString = "Pong";
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length, stoppingToken);
                context.Response.Close();
                _logger.LogInformation("Responded with {Response}", responseString);
            }
        }
    }
}
