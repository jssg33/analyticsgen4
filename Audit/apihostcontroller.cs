using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApihostEndpoints
{
    public static void MapApihostEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Apihosts")
            .WithTags(nameof(Apihost));

        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Apihosts.ToList();
            }
        })
        .WithName("GetAllApihosts")
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
                    context.Apihosts
                        .Where(x => x.Id == Id)
                        .ToList());
            }
        })
        .WithName("GetApihostById")
        .WithOpenApi();

        group.MapPost("/", async (Apihost input) =>
        {
            using (var context = new EnterpriseContext())
            {
                context.Apihosts.Add(input);

                await context.SaveChangesAsync();

                return TypedResults.Created(
                    $"Created ID:{input.Id}");
            }
        })
        .WithName("CreateApihost")
        .WithOpenApi();

        group.MapPut("/{id}",
        async (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["id"] ??
                httpContext.Request.RouteValues["Id"];

            var Id = Convert.ToInt32(idObj);

            var input =
                await httpContext.Request.ReadFromJsonAsync<Apihost>();

            using (var context = new EnterpriseContext())
            {
                var host =
                    context.Apihosts.FirstOrDefault(x => x.Id == Id);

                if (host == null)
                    return Results.NotFound();

                context.Apihosts.Attach(host);

                host.ApiHostName = input!.ApiHostName;
                host.ApiHostUrl = input.ApiHostUrl;
                host.SwaggerUsername = input.SwaggerUsername;
                host.SwaggerPassword = input.SwaggerPassword;
                host.IsSecure = input.IsSecure;
                host.LastAuditDate = input.LastAuditDate;
                host.LastAuditId = input.LastAuditId;
                host.Active = input.Active;

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Updated ID:{host.Id}");
            }
        })
        .WithName("UpdateApihost")
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
                var host =
                    context.Apihosts.FirstOrDefault(x => x.Id == Id);

                if (host == null)
                    return Results.NotFound();

                context.Apihosts.Remove(host);

                await context.SaveChangesAsync();

                return Results.Ok();
            }
        })
        .WithName("DeleteApihost")
        .WithOpenApi();
    }
}
