using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data.Configurations;

public static class SystemSettingConfigurations
{
    public static void ApplySystemSettingConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemSetting>().ToTable("SystemSettings");
        modelBuilder.Entity<SystemSetting>()
            .HasKey(f => f.Id);
        modelBuilder.Entity<SystemSetting>()
            .Property(f => f.Setting)
            .IsRequired();
        modelBuilder.Entity<SystemSetting>()
            .Property(f => f.Value)
            .IsRequired();
    }
}