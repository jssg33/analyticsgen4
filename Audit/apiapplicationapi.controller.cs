using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApplicationApiController
{
    public static void MapApplicationApiEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group =
            app.MapGroup("/api/ApplicationApi")
               .WithTags("ApplicationApi");

        // =====================================================
        // GET ALL
        // =====================================================

        group.MapGet("/",
            async () =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.ApplicationApis
                        .OrderBy(x => x.ApplicationId)
                        .ToListAsync());
            })
            .WithName("GetApplicationApis")
            .WithOpenApi();

        // =====================================================
        // GET BY ID
        // =====================================================

        group.MapGet("/{id}",
            async (int id) =>
            {
                using var context = new EnterpriseContext();

                var item =
                    await context.ApplicationApis
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                return Results.Ok(item);
            })
            .WithName("GetApplicationApi")
            .WithOpenApi();

        // =====================================================
        // GET BY APPLICATION
        // =====================================================

        group.MapGet("/application/{applicationId}",
            async (int applicationId) =>
            {
                using var context = new EnterpriseContext();

                var items =
                    await context.ApplicationApis
                        .Where(x => x.ApplicationId == applicationId)
                        .OrderBy(x => x.Id)
                        .ToListAsync();

                return Results.Ok(items);
            })
            .WithName("GetApplicationApisByApplication")
            .WithOpenApi();

        // =====================================================
        // GET BY HOST
        // =====================================================

        group.MapGet("/host/{apiHostId}",
            async (int apiHostId) =>
            {
                using var context = new EnterpriseContext();

                var items =
                    await context.ApplicationApis
                        .Where(x => x.ApiHostId == apiHostId)
                        .OrderBy(x => x.Id)
                        .ToListAsync();

                return Results.Ok(items);
            })
            .WithName("GetApplicationApisByHost")
            .WithOpenApi();

        // =====================================================
        // CREATE
        // =====================================================

        group.MapPost("/",
            async (ApplicationApi input) =>
            {
                using var context = new EnterpriseContext();

                if (input.CreatedDate == default)
                {
                    input.CreatedDate = DateTime.UtcNow;
                }

                context.ApplicationApis.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/ApplicationApi/{input.Id}",
                    input);
            })
            .WithName("CreateApplicationApi")
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
                        .ReadFromJsonAsync<ApplicationApi>();

                using var context = new EnterpriseContext();

                var item =
                    context.ApplicationApis
                        .FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                item.ApplicationId = input!.ApplicationId;
                item.ApiHostId = input.ApiHostId;

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Updated ID:{item.Id}");
            })
            .WithName("UpdateApplicationApi")
            .WithOpenApi();

        // =====================================================
        // DELETE
        // =====================================================

        group.MapDelete("/{id}",
            async (int id) =>
            {
                using var context = new EnterpriseContext();

                var item =
                    await context.ApplicationApis
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                context.ApplicationApis.Remove(item);

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Deleted ID:{id}");
            })
            .WithName("DeleteApplicationApi")
            .WithOpenApi();
    }
}
