using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;

namespace Security.Infrastructure.Data.Configurations;

public static class UserRoleConfiguration
{
    public static void ApplyUserRoleConfiguration(this ModelBuilder modelBuilder)
    {
        var ent = modelBuilder.Entity<UserRole>();
        ent.ToTable("RoleUser");
        ent.HasKey(f => new { f.UserId, f.RoleId });
    }
}