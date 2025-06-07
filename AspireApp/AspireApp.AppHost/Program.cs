using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Receiver>("receiver")
       .WithHttpEndpoint(targetPort: 5000);
builder.AddProject<Projects.Sender>("sender");
builder.AddProject<Projects.WebFrontend>("web");
builder.AddProject<Projects.LinearCongruentGenerator_API>("lcg")
       .WithHttpEndpoint(targetPort: 5010);

await builder.Build().RunAsync();
