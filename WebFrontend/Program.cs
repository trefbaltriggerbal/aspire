using Projects.WebFrontend.Components;
using Microsoft.Extensions.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole();
builder.AddServiceDefaults();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("lcg", (sp, client) =>
{
    var uriResolver = sp.GetRequiredService<IServiceUriResolver>();
    client.BaseAddress = uriResolver.Resolve("lcg");
});
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Projects.WebFrontend"));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
