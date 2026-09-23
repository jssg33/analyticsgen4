using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApplicationApiController
{
    public static void MapApplicationApiEndpoints(this WebApplication app)
    {
        app.MapGet("/api/ApplicationApi",
            async ([FromServices] EnterpriseContext db) =>
            {
                return await db.ApplicationApis
                    .OrderBy(x => x.ApplicationId)
                    .ToListAsync();
            })
            .WithTags("ApplicationApi");

        app.MapGet("/api/ApplicationApi/{id:int}",
            async (
                int id,
                [FromServices] EnterpriseContext db) =>
            {
                var item = await db.ApplicationApis.FindAsync(id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            })
            .WithTags("ApplicationApi");

        app.MapGet("/api/ApplicationApi/Application/{applicationId:int}",
            async (
                int applicationId,
                [FromServices] EnterpriseContext db) =>
            {
                var items = await db.ApplicationApis
                    .Where(x => x.ApplicationId == applicationId)
                    .ToListAsync();

                return Results.Ok(items);
            })
            .WithTags("ApplicationApi");

        app.MapGet("/api/ApplicationApi/ApiHost/{apiHostId:int}",
            async (
                int apiHostId,
                [FromServices] EnterpriseContext db) =>
            {
                var items = await db.ApplicationApis
                    .Where(x => x.ApiHostId == apiHostId)
                    .ToListAsync();

                return Results.Ok(items);
            })
            .WithTags("ApplicationApi");

        app.MapPost("/api/ApplicationApi",
            async (
                ApplicationApi item,
                [FromServices] EnterpriseContext db) =>
            {
                item.CreatedDate = DateTime.UtcNow;

                db.ApplicationApis.Add(item);

                await db.SaveChangesAsync();

                return Results.Created(
                    $"/api/ApplicationApi/{item.Id}",
                    item);
            })
            .WithTags("ApplicationApi");

        app.MapPut("/api/ApplicationApi/{id:int}",
            async (
                int id,
                ApplicationApi update,
                [FromServices] EnterpriseContext db) =>
            {
                var item = await db.ApplicationApis.FindAsync(id);

                if (item == null)
                    return Results.NotFound();

                item.ApplicationId = update.ApplicationId;
                item.ApiHostId = update.ApiHostId;

                await db.SaveChangesAsync();

                return Results.Ok(item);
            })
            .WithTags("ApplicationApi");

        app.MapDelete("/api/ApplicationApi/{id:int}",
            async (
                int id,
                [FromServices] EnterpriseContext db) =>
            {
                var item = await db.ApplicationApis.FindAsync(id);

                if (item == null)
                    return Results.NotFound();

                db.ApplicationApis.Remove(item);

                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithTags("ApplicationApi");
    }
}