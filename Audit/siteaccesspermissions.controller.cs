using Enterprise.Data;
using Enterprise.Models;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Controllers;

public static class SiteAccessPermissionsController
{
    public static void MapSiteAccessPermissionsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/siteaccesspermissions");

        group.MapGet("/", async (EnterpriseContext db) =>
        {
            return await db.SiteAccessPermissions.ToListAsync();
        })
        .WithName("01GetSiteAccessPermissions")
        .WithOpenApi();

        group.MapGet("/{id}", async (int id, EnterpriseContext db) =>
        {
            var item = await db.SiteAccessPermissions.FindAsync(id);

            return item is null
                ? Results.NotFound()
                : Results.Ok(item);
        })
        .WithName("02GetSiteAccessPermission")
        .WithOpenApi();

        group.MapPost("/", async (SiteAccessPermission permission, EnterpriseContext db) =>
        {
            db.SiteAccessPermissions.Add(permission);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/siteaccesspermissions/{permission.Id}",
                permission);
        })
        .WithName("03CreateSiteAccessPermission")
        .WithOpenApi();

        group.MapPut("/{id}", async (
            int id,
            SiteAccessPermission updated,
            EnterpriseContext db) =>
        {
            var existing = await db.SiteAccessPermissions.FindAsync(id);

            if (existing is null)
                return Results.NotFound();

            existing.UserId = updated.UserId;
            existing.SiteId = updated.SiteId;
            existing.AccessLevel = updated.AccessLevel;
            existing.IsEnabled = updated.IsEnabled;
            existing.GrantedBy = updated.GrantedBy;
            existing.GrantedDate = updated.GrantedDate;
            existing.ExpirationDate = updated.ExpirationDate;
            existing.ReviewedDate = updated.ReviewedDate;
            existing.ReviewedBy = updated.ReviewedBy;

            await db.SaveChangesAsync();

            return Results.Ok(existing);
        })
        .WithName("04UpdateSiteAccessPermission")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id, EnterpriseContext db) =>
        {
            var existing = await db.SiteAccessPermissions.FindAsync(id);

            if (existing is null)
                return Results.NotFound();

            db.SiteAccessPermissions.Remove(existing);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("05DeleteSiteAccessPermission")
        .WithOpenApi();
    }
}
