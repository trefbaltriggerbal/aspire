using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddJsonConsole();
builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("Projects.Receiver"));
builder.Services.AddHostedService<PingReceiverService>();

using var host = builder.Build();
host.Run();

