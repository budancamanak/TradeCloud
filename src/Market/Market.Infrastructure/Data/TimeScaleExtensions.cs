using Common.Core.Attributes;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Market.Infrastructure.Data;

public static class TimeScaleExtensions
{
    public static void ApplyHypertables(this MigrationBuilder migrationBuilder, params Type[] modelTypes)
    {
        foreach (var modelType in modelTypes)
        {
            // Use reflection to get the table name for the model (entity type)
            var tableName = modelType.Name + "s"; // You can modify this if you use custom table names in Fluent API

            // Loop through all properties in the entity model
            foreach (var property in modelType.GetProperties())
            {
                // Check if the property has the HypertableColumnAttribute
                if (property.GetCustomAttributes(typeof(HypertableColumnAttribute), false).Any())
                {
                    Console.WriteLine($"Marking table '{tableName}' as hypertable with column '{property.Name}'");

                    // Generate the SQL to create a hypertable on the specified column
                    migrationBuilder.Sql($"SELECT create_hypertable('\"{tableName}\"', '{property.Name}');");
                }
            }
        }
    }
}