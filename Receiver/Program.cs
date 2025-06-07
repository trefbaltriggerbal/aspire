using System.Net;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddJsonConsole();
    builder.AddOpenTelemetry();
});

var logger = loggerFactory.CreateLogger<Program>();

var listener = new HttpListener();
listener.Prefixes.Add("http://localhost:5000/");
listener.Start();
logger.LogInformation("Receiver listening on http://localhost:5000/");

while (true)
{
    var context = await listener.GetContextAsync();
    if (context.Request.Url?.AbsolutePath == "/ping")
    {
        var responseString = "Pong";
        var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer.Length;
        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        context.Response.Close();
        logger.LogInformation("Responded with {Response}", responseString);
    }
}
