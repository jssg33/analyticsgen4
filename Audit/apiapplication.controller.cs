using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApplicationController
{
    public static void MapApplicationEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group =
            app.MapGroup("/api/Application")
               .WithTags("Application");

        // =====================================================
        // GET ALL
        // =====================================================

        group.MapGet("/",
            async () =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.Applications
                        .OrderBy(x => x.ApplicationName)
                        .ToListAsync());
            })
            .WithName("01GetApplications")
            .WithOpenApi();

        // =====================================================
        // GET BY ID
        // =====================================================

        group.MapGet("/{id}",
            async (int id) =>
            {
                using var context = new EnterpriseContext();

                var item =
                    await context.Applications
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                return Results.Ok(item);
            })
            .WithName("GetApplication")
            .WithOpenApi();

        // =====================================================
        // CREATE
        // =====================================================

        group.MapPost("/",
            async (Application input) =>
            {
                using var context = new EnterpriseContext();

                if (input.CreatedDate == default)
                {
                    input.CreatedDate = DateTime.UtcNow;
                }

                context.Applications.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/Application/{input.Id}",
                    input);
            })
            .WithName("01CreateApplication")
            .WithOpenApi();

        // =====================================================
        // UPDATE
        // =====================================================

        group.MapPut("/{id}",
            async (HttpContext httpContext) =>
            {
                var idObj =
                    httpContext.Request.RouteValues["id"] ??
                    httpContext.Request.RouteValues["Id"];

                var id = Convert.ToInt32(idObj);

                var input =
                    await httpContext.Request
                        .ReadFromJsonAsync<Application>();

                using var context = new EnterpriseContext();

                var item =
                    context.Applications
                        .FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                item.ApplicationName = input!.ApplicationName;
                item.ApplicationCode = input.ApplicationCode;
                item.Description = input.Description;
                item.OwnerName = input.OwnerName;
                item.OwnerEmail = input.OwnerEmail;
                item.SupportGroup = input.SupportGroup;
                item.BusinessUnit = input.BusinessUnit;
                item.Environment = input.Environment;
                item.IsActive = input.IsActive;
                item.InventoryId = input.InventoryId;
                item.ModifiedDate = DateTime.UtcNow;

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Updated ID:{item.Id}");
            })
            .WithName("01UpdateApplication")
            .WithOpenApi();

        // =====================================================
        // DELETE
        // =====================================================

        group.MapDelete("/{id}",
            async (int id) =>
            {
                using var context = new EnterpriseContext();

                var item =
                    await context.Applications
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                context.Applications.Remove(item);

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Deleted ID:{id}");
            })
            .WithName("01DeleteApplication")
            .WithOpenApi();

        // =====================================================
        // GET APPLICATION APIS
        // =====================================================

        group.MapGet("/{id}/Apis",
            async (int id) =>
            {
                using var context = new EnterpriseContext();

                var application =
                    await context.Applications
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (application == null)
                    return Results.NotFound();

                var apis =
                    from aa in context.ApplicationApis
                    join host in context.Apihosts
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
            .WithName("01GetApplicationApis")
            .WithOpenApi();
    }
}