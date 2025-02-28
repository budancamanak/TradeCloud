using Market.Domain.Entities;
using Market.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Data;

public class MarketDbContext : DbContext
{
    public DbSet<Exchange> Exchanges { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Ticker> Tickers { get; set; }

    public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyExchangeTableConfigurations();
        modelBuilder.ApplyTickerTableConfigurations();
        modelBuilder.ApplyPriceTableConfigurations();
    }
}