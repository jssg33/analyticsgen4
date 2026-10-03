using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class TraderEndpoints
{
    public static void MapTraderEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Traders").WithTags(nameof(Trader));

        // GET ALL
        group.MapGet("/", () =>
        {
            using var context = new EnterpriseContext();
            return context.Traders.ToList();
        })
        .WithName("GetAllTraders")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{id}", (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var id = Convert.ToInt32(idObj);
            using var context = new EnterpriseContext();
            return Results.Ok(context.Traders.Where(t => t.Id == id).ToList());
        })
        .WithName("GetTraderById")
        .WithOpenApi();

        // CREATE
        group.MapPost("/", async (Trader input) =>
        {
            using var context = new EnterpriseContext();
            context.Traders.Add(input);
            await context.SaveChangesAsync();
            return TypedResults.Created("Created ID:" + input.Id);
        })
        .WithName("CreateTrader")
        .WithOpenApi();

        // UPDATE
        group.MapPut("/{id}", async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var id = Convert.ToInt32(idObj);
            var input = await httpContext.Request.ReadFromJsonAsync<Trader>();
            using var context = new EnterpriseContext();
            var entity = context.Traders.Where(t => t.Id == id).FirstOrDefault();
            if (entity == null) return Results.NotFound();

            context.Traders.Attach(entity);
            entity.FullName = input!.FullName;
            entity.StaffUserId = input.StaffUserId;
            entity.IsActive = input.IsActive;

            await context.SaveChangesAsync();
            return Results.Accepted($"Updated ID:{id}");
        })
        .WithName("UpdateTrader")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var id = Convert.ToInt32(idObj);
            using var context = new EnterpriseContext();
            var entity = context.Traders.Where(t => t.Id == id).FirstOrDefault();
            if (entity == null) return Results.NotFound();

            context.Traders.Attach(entity);
            context.Traders.Remove(entity);
            await context.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("DeleteTrader")
        .WithOpenApi();
    }
}
