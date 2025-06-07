using Projects.Codex;
using Projects.Codex.CLI;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole();
builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Projects.Codex.Api"));

builder.Services.AddSingleton(new CommandHandler(1103515245, 12345, 1L << 31, 1));

var app = builder.Build();

var activitySource = new ActivitySource("Projects.Codex.Api");

app.MapGet("/lcg/short", (CommandHandler handler, ILoggerFactory loggerFactory) =>
{
    using var activity = activitySource.StartActivity("short");
    var logger = loggerFactory.CreateLogger("LCG");
    var value = handler.Next();
    logger.LogInformation("short {Value}", value);
    return Results.Json(new { value });
});

app.MapGet("/lcg/fast/{steps:long}", (CommandHandler handler, long steps, ILoggerFactory loggerFactory) =>
{
    using var activity = activitySource.StartActivity("fast");
    var logger = loggerFactory.CreateLogger("LCG");
    try
    {
        handler.Jump(steps);
        logger.LogInformation("fast {Steps} -> {Seed}", steps, handler.Seed);
        return Results.Json(new { seed = handler.Seed });
    }
    catch (ArgumentException ex)
    {
        logger.LogWarning(ex, "fast failed");
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/lcg/set/{param}/{value:long}", (CommandHandler handler, string param, long value) =>
{
    return handler.Set(param, value) is { } error
        ? Results.BadRequest(new { error })
        : Results.Json(new { seed = handler.Seed });
});

app.MapGet("/lcg/get/{param}", (CommandHandler handler, string param) =>
{
    return handler.Get(param) is { } result
        ? Results.Json(new { result })
        : Results.BadRequest(new { error = "Unknown parameter" });
});

app.MapGet("/lcg/seed/{value:long}", (CommandHandler handler, long value) =>
{
    handler.SetSeed(value);
    return Results.Json(new { seed = handler.Seed });
});

app.MapDefaultEndpoints();

app.Run();
