using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class DatabaseTablesController
{
    public static void MapDatabaseTablesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/databasetables");

        // GET ALL
        group.MapGet("/", async () =>
        {
            using var context = new EnterpriseContext();

            var tables = await context.DatabaseTables
                .OrderBy(x => x.TableName)
                .ToListAsync();

            return Results.Ok(tables);
        })
        .WithName("GetDatabaseTables")
        .WithOpenApi();

        // DISCOVER EF TABLES
        group.MapPost("/discover", async () =>
        {
            using var context = new EnterpriseContext();

            var entities = context.Model
                .GetEntityTypes()
                .Where(x => x.GetTableName() != null)
                .ToList();

            int inserted = 0;
            int updated = 0;

            foreach (var entity in entities)
            {
                var tableName = entity.GetTableName();

                if (string.IsNullOrWhiteSpace(tableName))
                    continue;

                var schema = entity.GetSchema();

                var modelName = entity.ClrType.Name;

                var columnCount =
                    entity.GetProperties().Count();

                var existing =
                    await context.DatabaseTables
                        .FirstOrDefaultAsync(x =>
                            x.TableName == tableName);

                if (existing == null)
                {
                    context.DatabaseTables.Add(
                        new DatabaseTable
                        {
                            TableName = tableName,
                            SchemaName = schema,
                            ModelName = modelName,
                            ColumnCount = columnCount,
                            CreatedDate = DateTime.UtcNow,
                            LastAuditDate = DateTime.UtcNow,
                            IsActive = true
                        });

                    inserted++;
                }
                else
                {
                    existing.SchemaName = schema;
                    existing.ModelName = modelName;
                    existing.ColumnCount = columnCount;
                    existing.LastAuditDate = DateTime.UtcNow;

                    updated++;
                }
            }

            await context.SaveChangesAsync();

            return Results.Ok(new
            {
                TotalEntities = entities.Count,
                Inserted = inserted,
                Updated = updated,
                AuditDate = DateTime.UtcNow
            });
        })
        .WithName("DiscoverDatabaseTables")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var table = await context.DatabaseTables
                .FirstOrDefaultAsync(x => x.Id == id);

            return table == null
                ? Results.NotFound()
                : Results.Ok(table);
        })
        .WithName("GetDatabaseTable")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var table = await context.DatabaseTables
                .FirstOrDefaultAsync(x => x.Id == id);

            if (table == null)
                return Results.NotFound();

            context.DatabaseTables.Remove(table);

            await context.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("DeleteDatabaseTable")
        .WithOpenApi();

        // DISCOVERY
        group.MapGet("/discovery", async () =>
        {
            using var context = new EnterpriseContext();

            var count = await context.DatabaseTables.CountAsync();

            return Results.Ok(new
            {
                Controller = "DatabaseTablesController",
                Entity = "DatabaseTable",
                RecordCount = count,
                DiscoveryDate = DateTime.UtcNow
            });
        })
        .WithName("DatabaseTableDiscovery")
        .WithOpenApi();
    }
}