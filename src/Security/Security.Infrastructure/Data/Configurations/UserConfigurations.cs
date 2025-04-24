using Common.Core.Enums;
using Microsoft.EntityFrameworkCore;
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
        ent.Property(f => f.Status)
            .HasMaxLength(10)
            .HasConversion(
                v => v.Name,
                v => Status.FromName(v)!
            )
            .IsRequired();
        ent.HasMany(f => f.UserLogins).WithOne();
    }
}