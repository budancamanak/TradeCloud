using Backend.Domain.Entities;
using Common.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data.Configurations;

public static class PluginExecutionConfigurations
{
    public static void ApplyPluginExecutionConfigurations(this ModelBuilder modelBuilder)
    {
        var ent = modelBuilder.Entity<PluginExecution>();
        ent.ToTable("PluginExecutions");
        ent.HasKey(f => f.Id);
        ent.Property(f => f.Status)
            .HasMaxLength(20)
            .HasConversion(
                v => v.GetStringRepresentation(),
                v => v.ToPluginStatus()
            )
            .IsRequired();
        ent.Property(f => f.Error)
            .IsRequired(false);
        ent.Property(f => f.Progress)
            .HasDefaultValue(0.0)
            .IsRequired();
        ent.Property(f => f.ParamSet)
            .IsRequired();
        ent.HasMany(f => f.PluginOutputs)
            .WithOne(f => f.PluginExecution)
            .HasForeignKey(f => f.PluginId);
        ent.HasOne(f => f.AnalysisExecution)
            .WithMany(f => f.PluginExecutions)
            .HasForeignKey(f => f.AnalysisExecutionId);
        ent.HasMany(f => f.PluginOutputs)
            .WithOne(f => f.PluginExecution)
            .HasForeignKey(f => f.PluginId);
        ent.Property(f => f.QueuedDate)
            .IsRequired(false);
        ent.Property(f => f.FinishDate)
            .IsRequired(false);
        ent.Property(f => f.RunStartDate)
            .IsRequired(false);
    }
}