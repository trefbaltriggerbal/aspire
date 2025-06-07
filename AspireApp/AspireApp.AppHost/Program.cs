using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Receiver>("receiver");
builder.AddProject<Projects.Sender>("sender");
builder.AddProject<Projects.WebFrontend>("web");

await builder.Build().RunAsync();
