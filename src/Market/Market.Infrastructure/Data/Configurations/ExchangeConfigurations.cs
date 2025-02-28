using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Data.Configurations;

public static class ExchangeConfigurations
{
    public static void ApplyExchangeTableConfigurations(this ModelBuilder modelBuilder)
    {
        // Configure Exchange
        modelBuilder.Entity<Exchange>()
            .ToTable("Exchanges");

        modelBuilder.Entity<Exchange>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Exchange>()
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Exchange>()
            .Property(e => e.ConnectionUrl)
            .HasMaxLength(200); // Assuming a maximum length for ConnectionUrl

        modelBuilder.Entity<Exchange>()
            .HasMany(e => e.Tickers)
            .WithOne(t => t.Exchange)
            .HasForeignKey(t => t.ExchangeId);

        modelBuilder.Entity<Exchange>()
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}