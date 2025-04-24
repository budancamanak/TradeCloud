using Common.Security.Enums;
using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;

namespace Security.Infrastructure.Data.Configurations;

public static class PermissionConfigurations
{
    public static void ApplyPermissionConfigurations(this ModelBuilder modelBuilder)
    {
        var ent = modelBuilder.Entity<Permission>();
        ent.ToTable("Permissions");
        ent.HasKey(f => f.Id);
        ent.Property(f => f.Name).IsRequired();
        ent.HasData(Permissions.GetValues()
            .Select(s => new Permission
            {
                Id = s.Value,
                Name = s.Name
            }));
    }
}