using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;
using Security.Infrastructure.Data.Configurations;

namespace Security.Infrastructure.Data;

public class SecurityDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // optionsBuilder.UseLazyLoadingProxies();
        // todo enable seeding below
        // .UseSeeding()
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyUserConfigurations();
        modelBuilder.ApplyPermissionConfigurations();
        modelBuilder.ApplyRoleConfigurations();
        modelBuilder.ApplyRolePermissionConfigurations();
    }
}