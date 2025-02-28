using Microsoft.EntityFrameworkCore;

namespace Tests.Common.Database;

public abstract class DatabaseFixture<T> : IDisposable where T : DbContext
{
    public T DbContext { get; private set; }

    public DatabaseFixture(string databaseName)
    {
        var options = DatabaseOptionsFactory.CreateOptions<T>(GetDatabaseType(), databaseName);
        

        // Initialize DbContext
        DbContext = Activator.CreateInstance(typeof(T), options) as T;

        // Open and create the database schema in memory

        if (GetDatabaseType() != DbOptionsType.InMemory)
            DbContext.Database.OpenConnection();
        DbContext.Database.EnsureCreated();

        // Seed data here
        // SeedData();
    }

    protected abstract DbOptionsType GetDatabaseType();
    protected abstract void OneTimeSetUp();

    public abstract void SeedData();

    public void Dispose()
    {
        // Clean up
        DbContext?.Dispose();
    }
}