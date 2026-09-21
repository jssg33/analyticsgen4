using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class AllstockEndpoints
{
    public static void MapAllstockEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Allstock").WithTags(nameof(Allstock));

        // GET all
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Allstocks.ToList();
            }
        })
        .WithName("GetAllAllstocks")
        .WithOpenApi();

        // GET by ID
        group.MapGet("/{Id}", (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["Id"] ?? httpContext.Request.RouteValues["id"];
            var Id = Convert.ToInt32(idObj);
            using (var context = new EnterpriseContext())
            {
                return Results.Ok(context.Allstocks.Where(m => m.Id == Id).ToList());
            }
        })
        .WithName("GetAllstockById")
        .WithOpenApi();

    group.MapPut("/{id}", 
        async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var Id = Convert.ToInt32(idObj);
            var input = await httpContext.Request.ReadFromJsonAsync<Allstock>();
            using (var context = new EnterpriseContext())
            {
                var stock = context.Allstocks.FirstOrDefault(m => m.Id == Id);
                if (stock == null)
                    return Results.NotFound();

                context.Allstocks.Attach(stock);

                stock.Company = input!.Company;
                await context.SaveChangesAsync();
                return Results.Accepted($"Updated ID:{input.Id}");
            }
        })
        .WithName("UpdateAllstock")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (Allstock input) =>
        {
            using (var context = new EnterpriseContext())
            {
                context.Allstocks.Add(input);
                await context.SaveChangesAsync();
                return TypedResults.Created("Created ID:" + input.Id);
            }
        })
        .WithName("CreateAllstock")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{Id}", async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["Id"] ?? httpContext.Request.RouteValues["id"];
            var Id = Convert.ToInt32(idObj);
            using (var context = new EnterpriseContext())
            {
                var stock = context.Allstocks.FirstOrDefault(m => m.Id == Id);
                if (stock == null)
                    return Results.NotFound();

                context.Allstocks.Remove(stock);
                await context.SaveChangesAsync();
                return Results.Ok();
            }
        })
        .WithName("DeleteAllstock")
        .WithOpenApi();
    }
}
