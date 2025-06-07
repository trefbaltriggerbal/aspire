using Projects.WebFrontend.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole();
builder.AddServiceDefaults();
builder.Services.AddHttpClient();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Projects.WebFrontend"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();
builder.Services.AddSingleton<Projects.WebFrontend.Services.FakeUserDatabase>();
builder.Services.AddLogging();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/login", async (Projects.WebFrontend.Models.LoginRequest request, Projects.WebFrontend.Services.FakeUserDatabase db, HttpContext http, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("LoginEndpoint");
    if (db.TryValidate(request.Username, request.Password, out var claims))
    {
        var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        logger.LogInformation("User {Username} logged in", request.Username);
        return Results.Ok();
    }
    logger.LogWarning("Invalid login for {Username}", request.Username);
    return Results.Unauthorized();
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
