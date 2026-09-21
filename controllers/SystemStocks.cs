using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Enterprise.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Mvc;

namespace somecontrollers.Controllers;

public static class SystemStocksEndpoints
{
    public static void MapSystemStocksEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/SystemStocks").WithTags(nameof(SystemStock));

        // GET ALL
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.SystemStocks.ToList();
            }
        })
        .WithName("GetAllSystemStocks")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{id}", (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.SystemStocks.Where(m => m.Id == id).ToList();
            }
        })
        .WithName("GetSystemStockById")
        .WithOpenApi();

        // UPDATE
        group.MapPut("/{id}", async (int id, SystemStock input) =>
        {
            using (var context = new EnterpriseContext())
            {
                SystemStock[] stocks = context.SystemStocks.Where(m => m.Id == id).ToArray();

                context.SystemStocks.Attach(stocks[0]);

                stocks[0].Ticker = input.Ticker;
                stocks[0].CompanyName = input.CompanyName;

                // FIX: enum conversion
                stocks[0].Source = 1;
                
                stocks[0].Country = input.Country;
                stocks[0].PrimaryExchange = input.PrimaryExchange;
                stocks[0].SecondaryExchange = input.SecondaryExchange;

                await context.SaveChangesAsync();

                return TypedResults.Accepted("Updated ID:" + input.Id);
            }
        })
        .WithName("UpdateSystemStock")
        .WithOpenApi();

        // CREATE
        group.MapPost("/", async (SystemStock input) =>
        {
            using (var context = new EnterpriseContext())
            {
                context.SystemStocks.Add(input);
                await context.SaveChangesAsync();
                return TypedResults.Created("Created ID:" + input.Id);
            }
        })
        .WithName("CreateSystemStock")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                SystemStock[] stocks = context.SystemStocks.Where(m => m.Id == id).ToArray();

                context.SystemStocks.Attach(stocks[0]);
                context.SystemStocks.Remove(stocks[0]);

                await context.SaveChangesAsync();
            }

            return TypedResults.Ok("Deleted ID:" + id);
        })
        .WithName("DeleteSystemStock")
        .WithOpenApi();

        // BULK (NO LAMBDA)
        group.MapPost("/bulk", BulkCreate)
             .WithName("CreateSystemStocksBulk")
             .WithOpenApi();
    }

    // -------------------------
    // BULK HANDLER (NO LAMBDA)
    // -------------------------
    public static async Task<IResult> BulkCreate(HttpContext http, [FromBody] List<SystemStock> inputs)
    {
        if (inputs == null || inputs.Count == 0)
            return TypedResults.BadRequest("No stocks provided.");

        using (var context = new EnterpriseContext())
        {
            context.SystemStocks.AddRange(inputs);
            await context.SaveChangesAsync();

            return TypedResults.Created(
                "/api/SystemStocks/bulk",
                new { Count = inputs.Count, Ids = inputs.Select(s => s.Id).ToList() }
            );
        }
    }
}
