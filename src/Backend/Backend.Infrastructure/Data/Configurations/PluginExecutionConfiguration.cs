using Backend.Domain.Entities;
using Common.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data.Configurations;

public static class PluginExecutionConfiguration
{
    public static void ApplyPluginExecutionConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PluginExecution>().ToTable("PluginExecutions");
        modelBuilder.Entity<PluginExecution>().HasKey(f => f.Id);
        // modelBuilder.Entity<PluginExecution>()
        //     .Property(f => f.PluginIdentifier)
        //     .HasMaxLength(50)
        //     .IsRequired();
        // modelBuilder.Entity<PluginExecution>()
        //     .Property(f => f.TickerId)
        //     .IsRequired();
        modelBuilder.Entity<PluginExecution>()
            .Property(f => f.Status)
            .HasMaxLength(20)
            .HasConversion(
                v => v.GetStringRepresentation(),
                v => v.ToPluginStatus()
            )
            .IsRequired();
        // modelBuilder.Entity<PluginExecution>()
        //     .Property(p => p.Timeframe)
        //     .HasMaxLength(3)
        //     .HasConversion(
        //         v => v.GetStringRepresentation(),
        //         v => v.TimeFrameFromString()
        //     )
        //     .IsRequired();
        modelBuilder.Entity<PluginExecution>()
            .Property(f => f.Error)
            .IsRequired(false);
        modelBuilder.Entity<PluginExecution>()
            .Property(f => f.Progress)
            .HasDefaultValue(0.0)
            .IsRequired();
        modelBuilder.Entity<PluginExecution>()
            .Property(f => f.ParamSet)
            .IsRequired();
        // modelBuilder.Entity<PluginExecution>()
        //     .Property(f => f.UserId)
        //     .IsRequired();
        // modelBuilder.Entity<PluginExecution>()
        //     .Property(f => f.TradingParams)
        //     .IsRequired(false);
        // modelBuilder.Entity<PluginExecution>()
        //     .Property(f => f.CreatedDate)
        //     .IsRequired();
        // modelBuilder.Entity<PluginExecution>()
        //     .Property(f => f.StartDate)
        //     .IsRequired();
        // modelBuilder.Entity<PluginExecution>()
        //     .Property(f => f.EndDate)
        //     .IsRequired();

        modelBuilder.Entity<PluginExecution>()
            .HasMany(f => f.PluginOutputs)
            .WithOne(f => f.PluginExecution)
            .HasForeignKey(f => f.PluginId);
        modelBuilder.Entity<PluginExecution>()
            .HasOne(f => f.AnalysisExecution)
            .WithMany(f => f.PluginExecutions)
            .HasForeignKey(f => f.AnalysisExecutionId);
    }
}