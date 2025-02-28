using Backend.Domain.Entities;
using Common.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data.Configurations;

public static class PluginOutputConfigurations
{
    public static void ApplyPluginOutputConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PluginOutput>()
            .ToTable("PluginOutputs");
        modelBuilder.Entity<PluginOutput>()
            .HasKey(f => f.Id);
        modelBuilder.Entity<PluginOutput>()
            .Property(f => f.PluginId)
            .IsRequired();
        modelBuilder.Entity<PluginOutput>()
            .Property(f => f.PluginSignal)
            .HasMaxLength(20)
            .HasConversion(
                v => v.GetStringRepresentation(),
                v => v.ToSignalType()
            )
            .IsRequired();
        modelBuilder.Entity<PluginOutput>()
            .Property(f => f.SignalDate)
            .IsRequired();
        modelBuilder.Entity<PluginOutput>()
            .Property(f => f.CreatedDate)
            .IsRequired();
    }
}