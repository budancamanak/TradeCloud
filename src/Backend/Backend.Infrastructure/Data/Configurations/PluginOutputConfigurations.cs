using Backend.Domain.Entities;
using Common.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data.Configurations;

public static class PluginOutputConfigurations
{
    public static void ApplyPluginOutputConfigurations(this ModelBuilder modelBuilder)
    {
        var ent = modelBuilder.Entity<PluginOutput>();
        ent.ToTable("PluginOutputs");
        ent.HasKey(f => f.Id);
        ent.Property(f => f.PluginId).IsRequired();
        ent.Property(f => f.PluginSignal).HasMaxLength(20).HasConversion(
            v => v.GetStringRepresentation(),
            v => v.ToSignalType()
        ).IsRequired();
        ent.Property(f => f.SignalDate).IsRequired();
        ent.Property(f => f.CreatedDate).IsRequired();
    }
}