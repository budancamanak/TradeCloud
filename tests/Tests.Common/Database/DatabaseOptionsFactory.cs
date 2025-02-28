using Microsoft.EntityFrameworkCore;

namespace Tests.Common.Database;

public static class DatabaseOptionsFactory
{
    public static DbContextOptions<T> CreateOptions<T>(DbOptionsType type,string databaseName)
        where T : DbContext
    {
        return type switch
        {
            DbOptionsType.InMemory => InMemoryDbOptions.CreateOptions<T>(databaseName),
            DbOptionsType.SqLite => SqLiteInMemoryDbOptions.CreateOptions<T>(databaseName),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}

public enum DbOptionsType
{
    InMemory,
    SqLite
}