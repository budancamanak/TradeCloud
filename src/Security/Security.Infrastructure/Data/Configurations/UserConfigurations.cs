using Common.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Security.Domain.Entities;

namespace Security.Infrastructure.Data.Configurations;

public static class UserConfigurations
{
    public static void ApplyUserConfigurations(this ModelBuilder modelBuilder)
    {
        var ent = modelBuilder.Entity<User>();
        ent.ToTable("Users");
        ent.HasKey(f => f.Id);
        ent.Property(f => f.Username).HasMaxLength(50).IsRequired();
        ent.Property(f => f.Email).HasMaxLength(255).IsRequired();
        ent.Property(f => f.Password).IsRequired();
        ent.Property(f => f.CreatedDate).IsRequired();
        ent.Property(f => f.UserId).ValueGeneratedOnAdd().HasValueGenerator<UserIdGenerator>();
        ent.Property(f => f.Status)
            .HasMaxLength(10)
            .HasConversion(
                v => v.Name,
                v => Status.FromName(v)!
            )
            .IsRequired();
        ent.HasMany(f => f.UserLogins).WithOne(f => f.User);
        // ent.HasMany(f => f.UserRoles).WithMany(f=>f.Users).UsingEntity(f=>f.ToTable("RoleUser"));//.UsingEntity<Role>();
        ent.HasMany(f => f.UserRoles).WithMany().UsingEntity<UserRole>();
    }
}

public class UserIdGenerator : ValueGenerator<string>
{
    public override string Next(EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return Guid.NewGuid().ToString().Replace("-", "");
    }

    public override bool GeneratesTemporaryValues { get; }
}