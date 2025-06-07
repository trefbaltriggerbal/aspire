using System.Net.Http;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddJsonConsole();
    builder.AddOpenTelemetry();
});

var logger = loggerFactory.CreateLogger<Program>();
var client = new HttpClient();
logger.LogInformation("Sending request to receiver...");
var response = await client.GetStringAsync("http://localhost:5000/ping");
logger.LogInformation("Received response {Response}", response);
