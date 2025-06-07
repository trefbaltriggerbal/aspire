using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddJsonConsole();
builder.AddServiceDefaults();
builder.Services.AddHttpClient();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("Projects.Sender")
        .AddHttpClientInstrumentation());
builder.Services.AddHostedService<PingSenderService>();

using var host = builder.Build();
host.Run();
