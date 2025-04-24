using Backend.Domain.Entities;
using Common.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data.Configurations;

public static class AnalysisExecutionConfigurations
{
    public static void ApplyAnalysisExecutionConfigurations(this ModelBuilder modelBuilder)
    {
        var ent = modelBuilder.Entity<AnalysisExecution>();
        ent.ToTable("AnalysisExecutions");
        ent.HasKey(f => f.Id);
        ent.Property(f => f.PluginIdentifier)
            .HasMaxLength(50)
            .IsRequired();
        ent.Property(f => f.TickerId)
            .IsRequired();
        ent.Property(p => p.Timeframe)
            .HasMaxLength(3)
            .HasConversion(
                v => v.GetStringRepresentation(),
                v => v.TimeFrameFromString()
            )
            .IsRequired();
        ent.Property(f => f.ProgressCurrent)
            .HasDefaultValue(0.0)
            .IsRequired();
        ent.Property(f => f.ProgressTotal)
            .HasDefaultValue(0.0)
            .IsRequired();
        ent.Property(f => f.UserId)
            .IsRequired();
        ent.Property(f => f.ParamSet)
            .IsRequired();
        ent.Property(f => f.TradingParams)
            .IsRequired(false);
        ent.Property(f => f.CreatedDate)
            .IsRequired();
        ent.Property(f => f.StartDate)
            .IsRequired();
        ent.Property(f => f.EndDate)
            .IsRequired();
        ent.HasMany(f => f.PluginExecutions)
            .WithOne(f => f.AnalysisExecution)
            .HasForeignKey(f => f.AnalysisExecutionId);
    }
}