using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Data.Configurations;

public static class TickerConfigurations
{
    public static void ApplyTickerTableConfigurations(this ModelBuilder modelBuilder)
    {
        // Configure Ticker
        modelBuilder.Entity<Ticker>()
            .ToTable("Tickers");

        modelBuilder.Entity<Ticker>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Ticker>()
            .Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Ticker>()
            .Property(t => t.Symbol)
            .IsRequired()
            .HasMaxLength(25);

        modelBuilder.Entity<Ticker>()
            .Property(t => t.DecimalPoint)
            .IsRequired();

        modelBuilder.Entity<Ticker>()
            .HasOne(t => t.Exchange)
            .WithMany(e => e.Tickers)
            .HasForeignKey(t => t.ExchangeId);

        modelBuilder.Entity<Ticker>()
            .HasMany(t => t.Prices)
            .WithOne(p => p.Ticker)
            .HasForeignKey(p => p.TickerId);
    }
}