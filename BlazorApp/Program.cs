using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorApp.Components;
using BlazorApp.Components.Account;
using BlazorApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace BlazorApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        var useInMemory = builder.Configuration.GetValue<bool>("USE_INMEMORY_DB");
        var requireConfirmed = builder.Configuration.GetValue("REQUIRE_CONFIRMED_ACCOUNT", true);

        if (useInMemory)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("InMemoryDb"));
        }
        else
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseSqlServer(connectionString)
                    .EnableSensitiveDataLogging(true)
                    .EnableDetailedErrors(true)
            );
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        }

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = requireConfirmed)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        builder.Services.AddControllers();

        var supportedCultures = new[] { "en", "fr", "nl" };
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
            options.SupportedUICultures = options.SupportedCultures;
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (useInMemory)
            {
                db.Database.EnsureCreated();
            }
            else
            {
                db.Database.Migrate();
            }

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Client", "Parent", "Administrator" };
            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                }
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = userManager.FindByNameAsync("user").GetAwaiter().GetResult();
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = "user",
                    Email = "user@example.com",
                    EmailConfirmed = true
                };
                userManager.CreateAsync(user, "Abc123!").GetAwaiter().GetResult();
            }

            if (!userManager.IsInRoleAsync(user, "Client").GetAwaiter().GetResult())
            {
                userManager.AddToRoleAsync(user, "Client").GetAwaiter().GetResult();
            }

            bool alreadyHasPolicy = db.Policies.Any(p =>
                p.PolicyNumber == "POL123" && p.UserId == user.Id);

            if (!alreadyHasPolicy)
            {
                db.Policies.Add(new Policy
                {
                    UserId = user.Id,
                    PolicyNumber = "POL123",
                    Description = "Voorbeeldpolis",
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = DateTime.UtcNow.AddYears(1)
                });
                db.SaveChanges();        // sync variant
            }

            var policy = db.Policies.First(p => p.PolicyNumber == "POL123" && p.UserId == user.Id);
            if (!db.InsuranceClaims.Any(c => c.PolicyId == policy.Id))
            {
                db.InsuranceClaims.AddRange([
                    new InsuranceClaim
                    {
                        PolicyId = policy.Id,
                        Description = "Waterschade keuken",
                        IncidentDate = DateTime.UtcNow.AddDays(-10),
                        Status = "Open"
                    },
                    new InsuranceClaim
                    {
                        PolicyId = policy.Id,
                        Description = "Diefstal fiets",
                        IncidentDate = DateTime.UtcNow.AddDays(-30),
                        Status = "Closed"
                    }
                ]);
                db.SaveChanges();
            }

        }

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(localizationOptions);
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();
        app.MapControllers();

        app.Run();
    }
}
