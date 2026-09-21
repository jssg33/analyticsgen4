using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class AuditResultEndpoints
{
    public static void MapApiAuditResultEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/ApiAuditResults")
            .WithTags(nameof(AuditResult));

        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.AuditResults
                    .OrderByDescending(x => x.AuditDate)
                    .ToList();
            }
        })
        .WithName("GetAllAuditResults")
        .WithOpenApi();

        group.MapGet("/{Id}", (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["Id"] ??
                httpContext.Request.RouteValues["id"];

            var Id = Convert.ToInt32(idObj);

            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.AuditResults
                        .Where(x => x.Id == Id)
                        .ToList());
            }
        })
        .WithName("GetAuditResultById")
        .WithOpenApi();

        group.MapPost("/", async (AuditResult input) =>
        {
            using (var context = new EnterpriseContext())
            {
                context.AuditResults.Add(input);

                await context.SaveChangesAsync();

                return TypedResults.Created(
                    $"Created ID:{input.Id}");
            }
        })
        .WithName("CreateAuditResult")
        .WithOpenApi();

        group.MapPut("/{id}",
        async (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["id"] ??
                httpContext.Request.RouteValues["Id"];

            var Id = Convert.ToInt32(idObj);

            var input =
                await httpContext.Request.ReadFromJsonAsync<AuditResult>();

            using (var context = new EnterpriseContext())
            {
                var result =
                    context.AuditResults.FirstOrDefault(x => x.Id == Id);

                if (result == null)
                    return Results.NotFound();

                context.AuditResults.Attach(result);

                result.AuditDate = input!.AuditDate;
                result.TotalApiHosts = input.TotalApiHosts;
                result.ApiHostsSecure = input.ApiHostsSecure;
                result.ApiHostsInsecure = input.ApiHostsInsecure;
                result.TotalApiEndpoints = input.TotalApiEndpoints;
                result.TotalEndpointsSecure = input.TotalEndpointsSecure;
                result.TotalEndpointsInsecure = input.TotalEndpointsInsecure;
                result.AuditorId = input.AuditorId;
                result.AuditorName = input.AuditorName;
                result.Notes = input.Notes;

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Updated ID:{result.Id}");
            }
        })
        .WithName("UpdateAuditResult")
        .WithOpenApi();

        group.MapDelete("/{Id}",
        async (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["Id"] ??
                httpContext.Request.RouteValues["id"];

            var Id = Convert.ToInt32(idObj);

            using (var context = new EnterpriseContext())
            {
                var result =
                    context.AuditResults.FirstOrDefault(x => x.Id == Id);

                if (result == null)
                    return Results.NotFound();

                context.AuditResults.Remove(result);

                await context.SaveChangesAsync();

                return Results.Ok();
            }
        })
        .WithName("DeleteAuditResult")
        .WithOpenApi();
    }
}
