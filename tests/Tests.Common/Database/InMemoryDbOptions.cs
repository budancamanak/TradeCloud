using Microsoft.EntityFrameworkCore;

namespace Tests.Common.Database;

[Obsolete(message: "Using SQLite for in-memory db")]
public static class InMemoryDbOptions
{
    public static DbContextOptions<T> CreateOptions<T>(string databaseName) where T : DbContext
    {
        var options = new DbContextOptionsBuilder<T>()
            .UseInMemoryDatabase(databaseName: databaseName)
            // .UseNpgsql("Host=192.168.1.11;Port=5432;Username=postgres;Password=password;Database=postgres;Pooling=true;CommandTimeout=30;SSL Mode=Disable;")
            .Options;
        return options;
    }
}