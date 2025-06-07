using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Receiver>("receiver")
       .WithHttpEndpoint(targetPort: 5000);
builder.AddProject<Projects.Sender>("sender");
builder.AddProject<Projects.WebFrontend>("web");

await builder.Build().RunAsync();
