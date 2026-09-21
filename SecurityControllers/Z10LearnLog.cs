using Enterprise.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace somecontrollers.Controllers;

public static class LearnLogEndpoints
{
    public static void MapLearnLogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/LearnLog").WithTags(nameof(Learnlog));

        group.MapGet("/", async () =>
        {
            using var context = new EnterpriseContext();
            return await context.Learnlogs.AsNoTracking().ToListAsync();
        })
        .WithName("GetAllLearnLogs")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Learnlog>, NotFound>> (int id) =>
        {
            using var context = new EnterpriseContext();
            var learnLog = await context.Learnlogs.AsNoTracking()
                .FirstOrDefaultAsync(log => log.Id == id);

            return learnLog is null ? TypedResults.NotFound() : TypedResults.Ok(learnLog);
        })
        .WithName("GetLearnLogById")
        .WithOpenApi();

        group.MapPost("/", async Task<Created<Learnlog>> (Learnlog input) =>
        {
            using var context = new EnterpriseContext();
            context.Learnlogs.Add(input);
            await context.SaveChangesAsync();
            return TypedResults.Created($"/api/LearnLog/{input.Id}", input);
        })
        .WithName("CreateLearnLog")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<NoContent, NotFound>> (int id, Learnlog input) =>
        {
            using var context = new EnterpriseContext();
            var learnLog = await context.Learnlogs.FindAsync(id);
            if (learnLog is null)
            {
                return TypedResults.NotFound();
            }

            learnLog.Date = input.Date;
            learnLog.Description = input.Description;
            learnLog.uid = input.uid;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        })
        .WithName("UpdateLearnLog")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<NoContent, NotFound>> (int id) =>
        {
            using var context = new EnterpriseContext();
            var learnLog = await context.Learnlogs.FindAsync(id);
            if (learnLog is null)
            {
                return TypedResults.NotFound();
            }

            context.Learnlogs.Remove(learnLog);
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        })
        .WithName("DeleteLearnLog")
        .WithOpenApi();
    }
}