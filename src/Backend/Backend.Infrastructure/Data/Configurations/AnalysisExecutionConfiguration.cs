using Backend.Domain.Entities;
using Common.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data.Configurations;

public static class AnalysisExecutionConfiguration
{
    public static void ApplyAnalysisExecutionConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalysisExecution>().ToTable("AnalysisExecutions");
        modelBuilder.Entity<AnalysisExecution>().HasKey(f => f.Id);
        modelBuilder.Entity<AnalysisExecution>()
            .Property(f => f.PluginIdentifier)
            .HasMaxLength(50)
            .IsRequired();
        modelBuilder.Entity<AnalysisExecution>()
            .Property(f => f.TickerId)
            .IsRequired();
        modelBuilder.Entity<AnalysisExecution>()
            .Property(p => p.Timeframe)
            .HasMaxLength(3)
            .HasConversion(
                v => v.GetStringRepresentation(),
                v => v.TimeFrameFromString()
            )
            .IsRequired();
        modelBuilder.Entity<AnalysisExecution>()
            .Property(f => f.Progress)
            .HasDefaultValue(0.0)
            .IsRequired();
        modelBuilder.Entity<AnalysisExecution>()
            .Property(f => f.UserId)
            .IsRequired();
        modelBuilder.Entity<AnalysisExecution>()
            .Property(f => f.ParamSet)
            .IsRequired();
        modelBuilder.Entity<AnalysisExecution>()
            .Property(f => f.TradingParams)
            .IsRequired(false);
        modelBuilder.Entity<AnalysisExecution>()
            .Property(f => f.CreatedDate)
            .IsRequired();
        modelBuilder.Entity<AnalysisExecution>()
            .Property(f => f.StartDate)
            .IsRequired();
        modelBuilder.Entity<AnalysisExecution>()
            .Property(f => f.EndDate)
            .IsRequired();
    }
}