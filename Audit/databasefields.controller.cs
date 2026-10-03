using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class DatabaseFieldsController
{
    public static void MapDatabaseFieldsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/databasefields");

        // GET ALL
        group.MapGet("/", async () =>
        {
            using var context = new EnterpriseContext();

            var items = await context.DatabaseFields
                .OrderBy(x => x.TableName)
                .ThenBy(x => x.FieldName)
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("GetDatabaseFields")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var item = await context.DatabaseFields
                .FirstOrDefaultAsync(x => x.Id == id);

            return item == null
                ? Results.NotFound()
                : Results.Ok(item);
        })
        .WithName("GetDatabaseField")
        .WithOpenApi();

        // GET BY TABLE
        group.MapGet("/table/{tableName}", async (string tableName) =>
        {
            using var context = new EnterpriseContext();

            var items = await context.DatabaseFields
                .Where(x => x.TableName == tableName)
                .OrderBy(x => x.FieldName)
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("GetDatabaseFieldsByTable")
        .WithOpenApi();

        // POST
        group.MapPost("/", async (DatabaseField input) =>
        {
            using var context = new EnterpriseContext();

            input.CreatedDate = DateTime.UtcNow;

            context.DatabaseFields.Add(input);

            await context.SaveChangesAsync();

            return Results.Created(
                $"/api/databasefields/{input.Id}",
                input);
        })
        .WithName("CreateDatabaseField")
        .WithOpenApi();

        // PUT
        group.MapPut("/{id:int}", async (int id, DatabaseField input) =>
        {
            using var context = new EnterpriseContext();

            var existing = await context.DatabaseFields
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return Results.NotFound();

            existing.DatabaseTableId = input.DatabaseTableId;
            existing.TableName = input.TableName;
            existing.SchemaName = input.SchemaName;
            existing.FieldName = input.FieldName;
            existing.DataType = input.DataType;
            existing.ClrType = input.ClrType;
            existing.IsNullable = input.IsNullable;
            existing.MaxLength = input.MaxLength;
            existing.IsPrimaryKey = input.IsPrimaryKey;
            existing.LastAuditDate = DateTime.UtcNow;
            existing.IsActive = input.IsActive;

            await context.SaveChangesAsync();

            return Results.Ok(existing);
        })
        .WithName("UpdateDatabaseField")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var existing = await context.DatabaseFields
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return Results.NotFound();

            context.DatabaseFields.Remove(existing);

            await context.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("DeleteDatabaseField")
        .WithOpenApi();

        // DISCOVERY INFO
        group.MapGet("/discovery", async () =>
        {
            using var context = new EnterpriseContext();

            var count = await context.DatabaseFields.CountAsync();

            return Results.Ok(new
            {
                Controller = "DatabaseFieldsController",
                Entity = "DatabaseField",
                RecordCount = count,
                DiscoveryDate = DateTime.UtcNow
            });
        })
        .WithName("DatabaseFieldDiscovery")
        .WithOpenApi();
    }
}