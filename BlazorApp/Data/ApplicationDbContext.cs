using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<InsuranceClaim> InsuranceClaims => Set<InsuranceClaim>();
}
