using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApiServicesController
{
    public static void MapApiServicesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/apiservices");

        // GET ALL
        group.MapGet("/", async () =>
        {
            using var context = new EnterpriseContext();

            var items = await context.ApiServices
                .OrderBy(x => x.ServiceName)
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("GetApiServices")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var item = await context.ApiServices
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null
                ? Results.NotFound()
                : Results.Ok(item);
        })
        .WithName("GetApiService")
        .WithOpenApi();

        // POST
        group.MapPost("/", async (ApiService input) =>
        {
            using var context = new EnterpriseContext();

            input.CreatedDate = DateTime.UtcNow;

            context.ApiServices.Add(input);

            await context.SaveChangesAsync();

            return Results.Created(
                $"/api/apiservices/{input.Id}",
                input);
        })
        .WithName("CreateApiService")
        .WithOpenApi();

        // BULK POST
        group.MapPost("/bulk", async (List<ApiService> inputs) =>
        {
            if (inputs == null || !inputs.Any())
                return Results.BadRequest("No records supplied.");

            using var context = new EnterpriseContext();

            foreach (var item in inputs)
            {
                item.CreatedDate = DateTime.UtcNow;
            }

            context.ApiServices.AddRange(inputs);

            await context.SaveChangesAsync();

            return Results.Ok(inputs);
        })
        .WithName("CreateApiServicesBulk")
        .WithOpenApi();

        // PUT
        group.MapPut("/{id:int}", async (int id, ApiService input) =>
        {
            using var context = new EnterpriseContext();

            var existing = await context.ApiServices
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return Results.NotFound();

            existing.ServiceName = input.ServiceName;
            existing.InterfaceId = input.InterfaceId;
            existing.InterfaceName = input.InterfaceName;
            existing.ImplementationClass = input.ImplementationClass;
            existing.Description = input.Description;
            existing.ServiceType = input.ServiceType;
            existing.ServiceOwner = input.ServiceOwner;
            existing.ApiHostName = input.ApiHostName;
            existing.ApiHostId = input.ApiHostId;
            existing.IsActive = input.IsActive;
            existing.ModifiedDate = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Results.Ok(existing);
        })
        .WithName("UpdateApiService")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id:int}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var existing = await context.ApiServices
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return Results.NotFound();

            context.ApiServices.Remove(existing);

            await context.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("DeleteApiService")
        .WithOpenApi();

        // ACTIVE
        group.MapGet("/active/list", async () =>
        {
            using var context = new EnterpriseContext();

            var items = await context.ApiServices
                .Where(x => x.IsActive)
                .OrderBy(x => x.ServiceName)
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("GetActiveApiServices")
        .WithOpenApi();

        // DISCOVERY
        group.MapGet("/discovery", async () =>
        {
            using var context = new EnterpriseContext();

            var count = await context.ApiServices.CountAsync();

            return Results.Ok(new
            {
                Controller = nameof(ApiServicesController),
                Entity = nameof(ApiService),
                RecordCount = count,
                DiscoveryDate = DateTime.UtcNow
            });
        })
        .WithName("DiscoveryApiServices")
        .WithOpenApi();
    }
}