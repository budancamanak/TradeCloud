using Microsoft.EntityFrameworkCore;
using Security.Domain.Entities;

namespace Security.Infrastructure.Data.Configurations;

public static class UserLoginConfigurations
{
    public static void ApplyUserLoginConfigurations(this ModelBuilder modelBuilder)
    {
        var ent = modelBuilder.Entity<UserLogin>();
        ent.ToTable("UserLogins");
        ent.HasKey(f => f.Token);
        ent.Property(f => f.Token).IsRequired().ValueGeneratedNever();
        ent.Property(f => f.UserId).IsRequired();
        ent.Property(f => f.LoginDate).IsRequired();
        ent.Property(f => f.ExpirationDate).IsRequired();
        ent.Property(f => f.UserAgent).HasMaxLength(255);
        ent.Property(f => f.ClientIP).HasMaxLength(50);

        ent.HasOne(f => f.User).WithMany(f => f.UserLogins);
    }
}