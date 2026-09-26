using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class LunaLogEndpoints
{
    public static void MapLunaLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/lunalog")
            .WithTags("LunaLog");

        group.MapGet("/", async (
            int skip = 0,
            int take = 100) =>
        {
            using var context = new EnterpriseContext();

            return await context.LunaLogs
                .OrderByDescending(x => x.AccessTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        });

        group.MapGet("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var log = await context.LunaLogs.FindAsync(id);

            return log is not null
                ? Results.Ok(log)
                : Results.NotFound();
        });

        group.MapPost("/", async (Lunalog lunaLog) =>
        {
            using var context = new EnterpriseContext();

            if (lunaLog.AccessTime == null)
                lunaLog.AccessTime = DateTime.UtcNow;

            context.LunaLogs.Add(lunaLog);
            await context.SaveChangesAsync();

            return Results.Created(
                $"/api/lunalog/{lunaLog.Id}",
                lunaLog);
        });

        group.MapPut("/{id:int}", async (
            int id,
            Lunalog lunaLog) =>
        {
            using var context = new EnterpriseContext();

            if (id != lunaLog.Id)
                return Results.BadRequest();

            context.Entry(lunaLog).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await context.LunaLogs
                    .AnyAsync(e => e.Id == id);

                if (!exists)
                    return Results.NotFound();

                throw;
            }

            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var log = await context.LunaLogs.FindAsync(id);

            if (log is null)
                return Results.NotFound();

            context.LunaLogs.Remove(log);
            await context.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}