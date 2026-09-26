using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class UserProfileLogEndpoints
{
    public static void MapUserProfileLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/userprofilelog")
            .WithTags("UserProfileLog");

        group.MapGet("/", async (
            int skip = 0,
            int take = 100) =>
        {
            using var context = new EnterpriseContext();

            return await context.UserProfileLogs
                .OrderByDescending(x => x.DateCreated)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        });

        group.MapGet("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var profileLog = await context.UserProfileLogs.FindAsync(id);

            return profileLog is not null
                ? Results.Ok(profileLog)
                : Results.NotFound();
        });

        group.MapGet("/user/{uid:int}", async (
            int uid,
            int skip = 0,
            int take = 100) =>
        {
            using var context = new EnterpriseContext();

            var logs = await context.UserProfileLogs
                .Where(x => x.Uid == uid)
                .OrderByDescending(x => x.DateCreated)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return Results.Ok(logs);
        });

        group.MapPost("/", async (UserProfileLog profileLog) =>
        {
            using var context = new EnterpriseContext();

            if (profileLog.DateCreated == default)
                profileLog.DateCreated = DateTime.UtcNow;

            context.UserProfileLogs.Add(profileLog);
            await context.SaveChangesAsync();

            return Results.Created(
                $"/api/userprofilelog/{profileLog.Id}",
                profileLog);
        });

        group.MapPut("/{id:int}", async (
            int id,
            UserProfileLog profileLog) =>
        {
            using var context = new EnterpriseContext();

            if (id != profileLog.Id)
                return Results.BadRequest();

            context.Entry(profileLog).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await context.UserProfileLogs
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

            var profileLog = await context.UserProfileLogs.FindAsync(id);

            if (profileLog is null)
                return Results.NotFound();

            context.UserProfileLogs.Remove(profileLog);
            await context.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
