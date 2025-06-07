using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddJsonConsole();
        logging.AddOpenTelemetry();
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<PingReceiverService>();
    })
    .Build()
    .Run();
