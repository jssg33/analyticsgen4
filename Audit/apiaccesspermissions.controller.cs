using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApiAccessPermissionsController
{
    public static void MapApiAccessPermissionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/apiaccesspermissions");

        group.MapGet("/", async () =>
        {
            using var db = new EnterpriseContext();

            return await db.ApiAccessPermissions.ToListAsync();
        })
        .WithName("01GetApiAccessPermissions")
        .WithOpenApi();

        group.MapGet("/{id}", async (int id) =>
        {
            using var db = new EnterpriseContext();

            var item = await db.ApiAccessPermissions.FindAsync(id);

            return item is null
                ? Results.NotFound()
                : Results.Ok(item);
        })
        .WithName("02GetApiAccessPermission")
        .WithOpenApi();

        group.MapPost("/", async (ApiAccessPermission permission) =>
        {
            using var db = new EnterpriseContext();

            db.ApiAccessPermissions.Add(permission);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/apiaccesspermissions/{permission.Id}",
                permission);
        })
        .WithName("03CreateApiAccessPermission")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, ApiAccessPermission updated) =>
        {
            using var db = new EnterpriseContext();

            var existing = await db.ApiAccessPermissions.FindAsync(id);

            if (existing is null)
            {
                return Results.NotFound();
            }

            existing.UserId = updated.UserId;
            existing.ApiId = updated.ApiId;
            existing.PermissionLevel = updated.PermissionLevel;
            existing.IsEnabled = updated.IsEnabled;
            existing.GrantedBy = updated.GrantedBy;

            await db.SaveChangesAsync();

            return Results.Ok(existing);
        })
        .WithName("04UpdateApiAccessPermission")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using var db = new EnterpriseContext();

            var existing = await db.ApiAccessPermissions.FindAsync(id);

            if (existing is null)
            {
                return Results.NotFound();
            }

            db.ApiAccessPermissions.Remove(existing);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("05DeleteApiAccessPermission")
        .WithOpenApi();
    }
}