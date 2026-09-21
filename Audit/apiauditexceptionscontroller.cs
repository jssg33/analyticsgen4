using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class AuditExceptionEndpoints
{
    public static void MapApiAuditExceptionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/AuditExceptions")
            .WithTags(nameof(AuditException));

        // GET ALL
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.AuditExceptions.ToList();
            }
        })
        .WithName("GetAllAuditExceptions")
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
                    context.AuditExceptions
                        .Where(x => x.Id == Id)
                        .ToList());
            }
        })
        .WithName("GetAuditExceptionById")
        .WithOpenApi();

        // GET BY APIAUDITID
        group.MapGet("/audit/{ApiAuditId}", (int ApiAuditId) =>
        {
            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.AuditExceptions
                        .Where(x => x.ApiAuditId == ApiAuditId)
                        .ToList());
            }
        })
        .WithName("GetAuditExceptionsByApiAuditId")
        .WithOpenApi();

        // GET ACTIVE
        group.MapGet("/active", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.AuditExceptions
                        .Where(x => x.Active)
                        .ToList());
            }
        })
        .WithName("GetActiveAuditExceptions")
        .WithOpenApi();

        // CREATE
        group.MapPost("/", async (AuditException input) =>
        {
            using (var context = new EnterpriseContext())
            {
                context.AuditExceptions.Add(input);

                await context.SaveChangesAsync();

                return TypedResults.Created(
                    $"Created ID:{input.Id}");
            }
        })
        .WithName("CreateAuditException")
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
                await httpContext.Request.ReadFromJsonAsync<AuditException>();

            using (var context = new EnterpriseContext())
            {
                var exception =
                    context.AuditExceptions
                        .FirstOrDefault(x => x.Id == Id);

                if (exception == null)
                    return Results.NotFound();

                context.AuditExceptions.Attach(exception);

                exception.ApiAuditId = input!.ApiAuditId;
                exception.Reason = input.Reason;
                exception.ApprovedBy = input.ApprovedBy;
                exception.ApprovalDate = input.ApprovalDate;
                exception.ExpirationDate = input.ExpirationDate;
                exception.Active = input.Active;
                exception.Notes = input.Notes;

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Updated ID:{exception.Id}");
            }
        })
        .WithName("UpdateAuditException")
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
                var exception =
                    context.AuditExceptions
                        .FirstOrDefault(x => x.Id == Id);

                if (exception == null)
                    return Results.NotFound();

                context.AuditExceptions.Remove(exception);

                await context.SaveChangesAsync();

                return Results.Ok();
            }
        })
        .WithName("DeleteAuditException")
        .WithOpenApi();
    }
}
