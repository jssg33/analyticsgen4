using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class SysLogEndpoints
{
    public static void MapSysLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/syslog")
            .WithTags("SysLog");

        group.MapGet("/", async (
            int skip = 0,
            int take = 100) =>
        {
            using var context = new EnterpriseContext();

            return await context.Syslogs
                .OrderByDescending(x => x.LogDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        });

        group.MapGet("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var sysLog = await context.Syslogs.FindAsync(id);

            return sysLog is not null
                ? Results.Ok(sysLog)
                : Results.NotFound();
        });

        group.MapPost("/", async (Syslog sysLog) =>
        {
            using var context = new EnterpriseContext();

            if (sysLog.LogDate == default)
                sysLog.LogDate = DateTime.UtcNow;

            context.Syslogs.Add(sysLog);
            await context.SaveChangesAsync();

            return Results.Created(
                $"/api/syslog/{sysLog.Id}",
                sysLog);
        });

        group.MapPut("/{id:int}", async (
            int id,
            Syslog sysLog) =>
        {
            using var context = new EnterpriseContext();

            if (id != sysLog.Id)
                return Results.BadRequest();

            context.Entry(sysLog).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await context.Syslogs
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

            var sysLog = await context.Syslogs.FindAsync(id);

            if (sysLog is null)
                return Results.NotFound();

            context.Syslogs.Remove(sysLog);
            await context.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}