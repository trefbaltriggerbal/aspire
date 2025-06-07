using Projects.WebFrontend.Components;
using BzBusinessLogic;
using BzData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole();
builder.AddServiceDefaults();
builder.Services.AddHttpClient();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Projects.WebFrontend"));
builder.Services.AddDbContext<BzDbContext>(options =>
    options.UseInMemoryDatabase("bzdb"));
builder.Services.AddScoped<UserService>();

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
