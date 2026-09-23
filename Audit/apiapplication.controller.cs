using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApplicationController
{
    public static void MapApplicationEndpoints(this WebApplication app)
    {
        app.MapGet("/api/Application",
            async ([FromServices] EnterpriseContext db) =>
            {
                return await db.Applications
                    .OrderBy(x => x.ApplicationName)
                    .ToListAsync();
            })
            .WithTags("Application");

        app.MapGet("/api/Application/{id:int}",
            async (
                int id,
                [FromServices] EnterpriseContext db) =>
            {
                var item = await db.Applications.FindAsync(id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            })
            .WithTags("Application");

        app.MapPost("/api/Application",
            async (
                Application item,
                [FromServices] EnterpriseContext db) =>
            {
                item.CreatedDate = DateTime.UtcNow;

                db.Applications.Add(item);

                await db.SaveChangesAsync();

                return Results.Created(
                    $"/api/Application/{item.Id}",
                    item);
            })
            .WithTags("Application");

        app.MapPut("/api/Application/{id:int}",
            async (
                int id,
                Application update,
                [FromServices] EnterpriseContext db) =>
            {
                var item = await db.Applications.FindAsync(id);

                if (item == null)
                    return Results.NotFound();

                item.ApplicationName = update.ApplicationName;
                item.ApplicationCode = update.ApplicationCode;
                item.Description = update.Description;
                item.OwnerName = update.OwnerName;
                item.OwnerEmail = update.OwnerEmail;
                item.SupportGroup = update.SupportGroup;
                item.BusinessUnit = update.BusinessUnit;
                item.Environment = update.Environment;
                item.IsActive = update.IsActive;
                item.ModifiedDate = DateTime.UtcNow;

                await db.SaveChangesAsync();

                return Results.Ok(item);
            })
            .WithTags("Application");

        app.MapDelete("/api/Application/{id:int}",
            async (
                int id,
                [FromServices] EnterpriseContext db) =>
            {
                var item = await db.Applications.FindAsync(id);

                if (item == null)
                    return Results.NotFound();

                db.Applications.Remove(item);

                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithTags("Application");

        app.MapGet("/api/Application/{id:int}/Apis",
            async (
                int id,
                [FromServices] EnterpriseContext db) =>
            {
                var application = await db.Applications
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (application == null)
                    return Results.NotFound();

                var apis =
                    from aa in db.ApplicationApis
                    join host in db.Apihosts
                        on aa.ApiHostId equals host.Id
                    where aa.ApplicationId == id
                    select new
                    {
                        host.Id,
                        host.ApiHostName,
                        host.ApiHostUrl,
                        host.Environment,
                        host.LastAuditDate,
                        host.Active
                    };

                return Results.Ok(new
                {
                    application.Id,
                    application.ApplicationName,
                    Apis = await apis.ToListAsync()
                });
            })
            .WithTags("Application");
    }
}