using System;
using System.Linq;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApiInterfacesAuditEndpoints
{
    public static void MapApiInterfacesAuditEndpoints(
        this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/ApiInterfacesAudit")
            .WithTags(nameof(ApiInterfacesAudit));

        // GET ALL
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.ApiInterfacesAudit.ToList();
            }
        })
        .WithName("GetAllApiInterfacesAudit")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{Id}", (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["Id"] ??
                httpContext.Request.RouteValues["id"];

            var Id = Convert.ToInt32(idObj);

            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.ApiInterfacesAudit
                        .Where(x => x.Id == Id)
                        .ToList());
            }
        })
        .WithName("GetApiInterfacesAuditById")
        .WithOpenApi();

        // GET BY APIAUDITID
        group.MapGet("/audit/{ApiAuditId}", (int ApiAuditId) =>
        {
            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.ApiInterfacesAudit
                        .Where(x => x.ApiAuditId == ApiAuditId)
                        .ToList());
            }
        })
        .WithName("GetApiInterfacesAuditByApiAuditId")
        .WithOpenApi();

        // GET BY INTERFACE
        group.MapGet("/interface/{InterfaceName}",
            (string InterfaceName) =>
        {
            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.ApiInterfacesAudit
                        .Where(x => x.InterfaceName == InterfaceName)
                        .ToList());
            }
        })
        .WithName("GetApiInterfacesAuditByInterface")
        .WithOpenApi();

        // CREATE
        group.MapPost("/", async (ApiInterfacesAudit input) =>
        {
            using (var context = new EnterpriseContext())
            {
                input.DiscoveredDate = DateTime.UtcNow;

                context.ApiInterfacesAudit.Add(input);

                await context.SaveChangesAsync();

                return TypedResults.Created(
                    $"Created ID:{input.Id}");
            }
        })
        .WithName("CreateApiInterfacesAudit")
        .WithOpenApi();

        // UPDATE
        group.MapPut("/{id}",
        async (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["id"] ??
                httpContext.Request.RouteValues["Id"];

            var Id = Convert.ToInt32(idObj);

            var input =
                await httpContext.Request
                    .ReadFromJsonAsync<ApiInterfacesAudit>();

            using (var context = new EnterpriseContext())
            {
                var record =
                    context.ApiInterfacesAudit
                        .FirstOrDefault(x => x.Id == Id);

                if (record == null)
                    return Results.NotFound();

                context.ApiInterfacesAudit.Attach(record);

                record.ApiAuditId = input!.ApiAuditId;
                record.ControllerName = input.ControllerName;
                record.Route = input.Route;
                record.HttpMethod = input.HttpMethod;
                record.InterfaceName = input.InterfaceName;
                record.ImplementationName = input.ImplementationName;
                record.ServiceLifetime = input.ServiceLifetime;
                record.IsRegistered = input.IsRegistered;
                record.MethodName = input.MethodName;

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Updated ID:{record.Id}");
            }
        })
        .WithName("UpdateApiInterfacesAudit")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{Id}",
        async (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["Id"] ??
                httpContext.Request.RouteValues["id"];

            var Id = Convert.ToInt32(idObj);

            using (var context = new EnterpriseContext())
            {
                var record =
                    context.ApiInterfacesAudit
                        .FirstOrDefault(x => x.Id == Id);

                if (record == null)
                    return Results.NotFound();

                context.ApiInterfacesAudit.Remove(record);

                await context.SaveChangesAsync();

                return Results.Ok();
            }
        })
        .WithName("DeleteApiInterfacesAudit")
        .WithOpenApi();
    }
}