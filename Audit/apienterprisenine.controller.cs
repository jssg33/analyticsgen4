using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace Enterprise.Controllers;

public static class Enterprise9Controller
{
    public static void MapEnterprise9Endpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/enterprise9");

        // GET ALL
        group.MapGet("/", async () =>
        {
            using var context = new EnterpriseContext();

            var items = await context.Enterprise9s
                .OrderBy(x => x.ApiHostName)
                .ThenBy(x => x.LogName)
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("GetEnterprise9")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var item = await context.Enterprise9s
                .FirstOrDefaultAsync(x => x.Id == id);

            return item == null
                ? Results.NotFound()
                : Results.Ok(item);
        })
        .WithName("GetEnterprise9ById")
        .WithOpenApi();

        // GET BY HOST ID
        group.MapGet("/host/{apiHostId:int}", async (int apiHostId) =>
        {
            using var context = new EnterpriseContext();

            var items = await context.Enterprise9s
                .Where(x => x.ApiHostId == apiHostId)
                .OrderBy(x => x.LogName)
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("GetEnterprise9ByHost")
        .WithOpenApi();

        // GET REQUIRED
        group.MapGet("/required", async () =>
        {
            using var context = new EnterpriseContext();

            var items = await context.Enterprise9s
                .Where(x => x.IsRequired)
                .OrderBy(x => x.LogName)
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("GetRequiredEnterprise9")
        .WithOpenApi();

        // POST
        group.MapPost("/", async (Enterprise9 input) =>
        {
            using var context = new EnterpriseContext();

            input.CreatedDate = DateTime.UtcNow;

            context.Enterprise9s.Add(input);

            await context.SaveChangesAsync();

            return Results.Created(
                $"/api/enterprise9/{input.Id}",
                input);
        })
        .WithName("CreateEnterprise9")
        .WithOpenApi();

        // BULK POST
        group.MapPost("/bulk", async (List<Enterprise9> inputs) =>
        {
            if (inputs == null || !inputs.Any())
                return Results.BadRequest("No records supplied.");

            using var context = new EnterpriseContext();

            foreach (var item in inputs)
            {
                item.CreatedDate = DateTime.UtcNow;
            }

            context.Enterprise9s.AddRange(inputs);

            await context.SaveChangesAsync();

            return Results.Ok(inputs);
        })
        .WithName("CreateEnterprise9Bulk")
        .WithOpenApi();

        // PUT
        group.MapPut("/{id:int}", async (int id, Enterprise9 input) =>
        {
            using var context = new EnterpriseContext();

            var existing = await context.Enterprise9s
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return Results.NotFound();

            existing.ApiHostId = input.ApiHostId;
            existing.ApiHostName = input.ApiHostName;
            existing.LogName = input.LogName;
            existing.LogDescription = input.LogDescription;
            existing.EndpointUrl = input.EndpointUrl;
            existing.HttpMethod = input.HttpMethod;
            existing.IsRequired = input.IsRequired;
            existing.IsImplemented = input.IsImplemented;
            existing.LastValidated = input.LastValidated;
            existing.Notes = input.Notes;
            existing.IsActive = input.IsActive;
            existing.ModifiedDate = DateTime.UtcNow;
            existing.ApiServiceId = input.ApiServiceId;
            existing.ApiServiceName = input.ApiServiceName;
            await context.SaveChangesAsync();

            return Results.Ok(existing);
        })
        .WithName("UpdateEnterprise9")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var existing = await context.Enterprise9s
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return Results.NotFound();

            context.Enterprise9s.Remove(existing);

            await context.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("DeleteEnterprise9")
        .WithOpenApi();

        // DISCOVERY
        group.MapGet("/discovery", async () =>
        {
            using var context = new EnterpriseContext();

            var count = await context.Enterprise9s.CountAsync();

            return Results.Ok(new
            {
                Controller = "Enterprise9Controller",
                Entity = "Enterprise9",
                RecordCount = count,
                DiscoveryDate = DateTime.UtcNow
            });
        })
        .WithName("DiscoveryEnterprise9")
        .WithOpenApi();

        // VALIDATION SUMMARY
        group.MapGet("/host/{apiHostId:int}/summary", async (int apiHostId) =>
        {
            using var context = new EnterpriseContext();

            var logs = await context.Enterprise9s
                .Where(x => x.ApiHostId == apiHostId)
                .ToListAsync();

            return Results.Ok(new
            {
                ApiHostId = apiHostId,
                TotalLogs = logs.Count,
                RequiredLogs = logs.Count(x => x.IsRequired),
                ImplementedLogs = logs.Count(x => x.IsImplemented),
                MissingLogs = logs.Count(x => x.IsRequired && !x.IsImplemented)
            });
        })
        .WithName("Enterprise9Summary")
        .WithOpenApi();
    }
}