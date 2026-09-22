using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

    public static class ApiHostExceptionController
    {
        public static void MapApiHostExceptionEndpoints(
            this IEndpointRouteBuilder app)
        {
            var group =
                app.MapGroup("/api/ApiHostException")
                   .WithTags("ApiHostException");

            // =====================================================
            // GET ALL
            // =====================================================

            group.MapGet("/",
                async () =>
                {
                    using var context = new EnterpriseContext();

                    return Results.Ok(
                        await context.ApiHostExceptions
                            .OrderBy(x => x.Id).ToListAsync());
                })
                .WithName("GetApiHostExceptions")
                .WithOpenApi();

            // =====================================================
            // GET BY ID
            // =====================================================

            group.MapGet("/{id}",
                async (int id) =>
                {
                    using var context = new EnterpriseContext();

                    var item =
                        await context.ApiHostExceptions.FirstOrDefaultAsync(x => x.Id == id);

                    if (item == null)
                        return Results.NotFound();

                    return Results.Ok(item);
                })
                .WithName("GetApiHostException")
                .WithOpenApi();

            // =====================================================
            // GET BY HOST ID
            // =====================================================

            group.MapGet("/host/{apiHostId}",
                async (int apiHostId) =>
                {
                    using var context = new EnterpriseContext();

                    var results =
                        await context.ApiHostExceptions
                            .Where(x => x.ApiHostId == apiHostId)
                            .OrderBy(x => x.Id)
                            .ToListAsync();

                    return Results.Ok(results);
                })
                .WithName("GetApiHostExceptionsByHost")
                .WithOpenApi();

            // =====================================================
            // CREATE
            // =====================================================

            group.MapPost("/",
                async (ApiHostException input) =>
                {
                    using var context = new EnterpriseContext();

                    if (input.CreatedDate == default)
                    {
                        input.CreatedDate = DateTime.UtcNow;
                    }

                    context.ApiHostExceptions.Add(input);

                    await context.SaveChangesAsync();

                    return Results.Created(
                        $"/api/ApiHostException/{input.Id}",
                        input);
                })
                .WithName("CreateApiHostException")
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
                            .ReadFromJsonAsync<ApiHostException>();

                    using var context = new EnterpriseContext();

                    var item =
                        context.ApiHostExceptions
                            .FirstOrDefault(x => x.Id == id);

                    if (item == null)
                        return Results.NotFound();

                    item.ApiHostId = input!.ApiHostId;
                    item.ExceptionType = input.ExceptionType;
                    item.Reason = input.Reason;
                    item.ApprovedBy = input.ApprovedBy;
                    item.ApprovalDate = input.ApprovalDate;
                    item.ExpirationDate = input.ExpirationDate;
                    item.Active = input.Active;
                    item.Notes = input.Notes;

                    await context.SaveChangesAsync();

                    return Results.Accepted(
                        $"Updated ID:{item.Id}");
                })
                .WithName("UpdateApiHostException")
                .WithOpenApi();

            // =====================================================
            // DELETE
            // =====================================================

            group.MapDelete("/{id}",
                async (int id) =>
                {
                    using var context = new EnterpriseContext();

                    var item =
                        await context.ApiHostExceptions
                            .FirstOrDefaultAsync(x => x.Id == id);

                    if (item == null)
                        return Results.NotFound();

                    context.ApiHostExceptions.Remove(item);

                    await context.SaveChangesAsync();

                    return Results.Accepted(
                        $"Deleted ID:{id}");
                })
                .WithName("DeleteApiHostException")
                .WithOpenApi();
        }
    }
