using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class TradeOrdersEndpoints
{
    public static void MapTradeOrdersEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TradeOrders").WithTags(nameof(TradeOrders));

        // GET ALL
        group.MapGet("/", () =>
        {
            using var context = new EnterpriseContext();
            return context.TradeOrders.ToList();
        })
        .WithName("GetAllTradeOrders")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{id}", (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var id = Convert.ToInt32(idObj);
            using var context = new EnterpriseContext();
            return Results.Ok(context.TradeOrders.Where(o => o.Id == id).ToList());
        })
        .WithName("GetTradeOrdersById")
        .WithOpenApi();

        // CREATE
        group.MapPost("/", async (TradeOrder input) =>
        {
            using var context = new EnterpriseContext();
            context.TradeOrders.Add(input);
            await context.SaveChangesAsync();
            return TypedResults.Created("Created ID:" + input.Id);
        })
        .WithName("CreateTradeOrders")
        .WithOpenApi();

        // UPDATE
        group.MapPut("/{id}", async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var id = Convert.ToInt32(idObj);
            var input = await httpContext.Request.ReadFromJsonAsync<TradeOrder>();
            using var context = new EnterpriseContext();
            var entity = context.TradeOrders.Where(o => o.Id == id).FirstOrDefault();
            if (entity == null) return Results.NotFound();

            context.TradeOrders.Attach(entity);

            entity.PortfolioId = input!.PortfolioId;
            entity.PortfolioStockId = input.PortfolioStockId;
            entity.TraderId = input.TraderId;
            entity.Quantity = input.Quantity;
            entity.ExecutionPrice = input.ExecutionPrice;
            entity.ExecutedAt = input.ExecutedAt;
            entity.OrderType = input.OrderType;

            await context.SaveChangesAsync();
            return Results.Accepted($"Updated ID:{id}");
        })
        .WithName("UpdateTradeOrders")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var id = Convert.ToInt32(idObj);
            using var context = new EnterpriseContext();
            var entity = context.TradeOrders.Where(o => o.Id == id).FirstOrDefault();
            if (entity == null) return Results.NotFound();

            context.TradeOrders.Attach(entity);
            context.TradeOrders.Remove(entity);
            await context.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("DeleteTradeOrders")
        .WithOpenApi();
    }
}
