using Common.Core.Enums;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Data.Configurations;

public static class PriceConfigurations
{
    public static void ApplyPriceTableConfigurations(this ModelBuilder modelBuilder)
    {
        // Configure Price (ChartData)
        modelBuilder.Entity<Price>()
            .ToTable("Prices");

        modelBuilder.Entity<Price>()
            .HasKey(p => new { p.Timestamp, p.TickerId, p.Timeframe });

        modelBuilder.Entity<Price>()
            .Property(p => p.Timestamp)
            .IsRequired();

        modelBuilder.Entity<Price>()
            .Property(p => p.High)
            .IsRequired();

        modelBuilder.Entity<Price>()
            .Property(p => p.Low)
            .IsRequired();

        modelBuilder.Entity<Price>()
            .Property(p => p.Open)
            .IsRequired();

        modelBuilder.Entity<Price>()
            .Property(p => p.Close)
            .IsRequired();

        modelBuilder.Entity<Price>()
            .Property(p => p.TickerId)
            .IsRequired();

        modelBuilder.Entity<Price>()
            .Property(p => p.Timeframe)
            .HasMaxLength(3)
            .HasConversion(
                v => v.GetStringRepresentation(),
                v => v.TimeFrameFromString()
            )
            .IsRequired();

        modelBuilder.Entity<Price>()
            .HasOne(p => p.Ticker)
            .WithMany(t => t.Prices)
            .HasForeignKey(p => p.TickerId);
    }
}