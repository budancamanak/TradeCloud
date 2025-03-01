using Tests.Common.Data;
using Market.Infrastructure.Data;
using Tests.Common.Database;

namespace Infrastructure.Tests;

public class MarketDatabaseFixture : DatabaseFixture<MarketDbContext>
{
    public MarketDatabaseFixture(string databaseName) : base(databaseName)
    {
    }

    protected override void OneTimeSetUp()
    {
        SeedData();
    }

    protected override DbOptionsType GetDatabaseType()
    {
        return DbOptionsType.SqLite;
    }

    public override void SeedData()
    {
        if (!DbContext.Exchanges.Any())
        {
            DbContext.Exchanges.AddRange(MarketServiceTestData.Instance.Exchanges);
            DbContext.SaveChanges();
        }
    }
}