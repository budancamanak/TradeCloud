using Common.Security.Enums;
using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;

namespace Security.Infrastructure.Data.Configurations;

public static class RoleConfigurations
{
    public static void ApplyRoleConfigurations(this ModelBuilder modelBuilder)
    {
        var ent = modelBuilder.Entity<Role>();
        ent.ToTable("Roles");
        ent.HasKey(f => f.Id);
        ent.Property(f => f.Name).IsRequired();
        ent.HasMany(f => f.Permissions).WithMany().UsingEntity<RolePermission>();
        ent.HasMany(f => f.Users).WithMany();
        ent.HasData(Roles.GetValues()
            .Select(s => new Role
            {
                Id = s.Value,
                Name = s.Name
            }));
    }
}