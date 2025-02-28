using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests.Common.Database;

public static class SqLiteInMemoryDbOptions
{
    public static DbContextOptions<T> CreateOptions<T>(string dbName) where T : DbContext
    {
        //This creates the SQLite connection string to in-memory database
        var connectionStringBuilder = new SqliteConnectionStringBuilder
            { DataSource = ":memory:" };
        var connectionString = connectionStringBuilder.ToString();

        //This creates a SqliteConnectionwith that string
        var connection = new SqliteConnection(connectionString);

        //The connection MUST be opened here
        connection.Open();

        //Now we have the EF Core commands to create SQLite options
        var builder = new DbContextOptionsBuilder<T>();
        builder.UseSqlite(connection);

        return builder.Options;
    }
}