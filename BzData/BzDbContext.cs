using BzData.Entities;
using Microsoft.EntityFrameworkCore;

namespace BzData;

public class BzDbContext : DbContext
{
    public BzDbContext(DbContextOptions<BzDbContext> options) : base(options)
    {
    }

    public DbSet<BzUser> Users => Set<BzUser>();
}
