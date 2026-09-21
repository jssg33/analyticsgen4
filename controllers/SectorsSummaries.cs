using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class SectorssummaryEndpoints
{
    public static void MapSectorssummaryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Sectorsummaries").WithTags(nameof(Sectorssummary));

        // GET all
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Sectorssummaries.ToList();
            }
        })
        .WithName("GetAllSectorsummaries")
        .WithOpenApi();

        // GET by ID
        group.MapGet("/{Id}", (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["Id"] ?? httpContext.Request.RouteValues["id"];
            var Id = Convert.ToInt32(idObj);
            using (var context = new EnterpriseContext())
            {
                return Results.Ok(context.Sectorssummaries.Where(m => m.Id == Id).ToList());
            }
        })
        .WithName("GetSectorssummaryById")
        .WithOpenApi();

group.MapPut("/{Id}",
    async (HttpContext httpContext) =>
{
    var idObj = httpContext.Request.RouteValues["Id"] ?? httpContext.Request.RouteValues["id"];
    var Id = Convert.ToInt32(idObj);
    var input = await httpContext.Request.ReadFromJsonAsync<Sectorssummary>();
    if (input == null)
        return Results.BadRequest("Invalid Sectorssummary payload");
    using (var context = new EnterpriseContext())
    {
        var summary = context.Sectorssummaries.FirstOrDefault(m => m.Id == Id);
        if (summary == null)
            return Results.NotFound();

        context.Sectorssummaries.Attach(summary);

        // Only update fields that are NOT null
        void UpdateIfNotNull<T>(Action<T> setter, object? value)
        {
            if (value is T v)
                setter(v);
        }

        // STRING FIELDS
        UpdateIfNotNull((string v) => summary.Company = v, input!.Company);
        UpdateIfNotNull((string v) => summary.Ticker = v, input.Ticker);
        UpdateIfNotNull((string v) => summary.Sector = v, input.Sector);
        UpdateIfNotNull((string v) => summary.Selected = v, input.Selected);

        // DOUBLE? FIELDS (MATCH YOUR MODEL)
        UpdateIfNotNull((double? v) => summary.Avgdividend = v, input.Avgdividend);
        UpdateIfNotNull((double? v) => summary.Totaldividends = v, input.Totaldividends);
        UpdateIfNotNull((double? v) => summary.PriceStart = v, input.PriceStart);
        UpdateIfNotNull((double? v) => summary.PriceEnd = v, input.PriceEnd);
        UpdateIfNotNull((double? v) => summary.Change = v, input.Change);
        UpdateIfNotNull((double? v) => summary.Totalreturn = v, input.Totalreturn);
        UpdateIfNotNull((double? v) => summary.Totalreturnover10 = v, input.Totalreturnover10);
        UpdateIfNotNull((double? v) => summary.Shares500 = v, input.Shares500);
        UpdateIfNotNull((double? v) => summary.Totalspend = v, input.Totalspend);
        UpdateIfNotNull((double? v) => summary.Fiveyearequityproj = v, input.Fiveyearequityproj);
        UpdateIfNotNull((double? v) => summary.Fiveyeardivproj = v, input.Fiveyeardivproj);
        UpdateIfNotNull((double? v) => summary.Totalfiveyearview = v, input.Totalfiveyearview);

        await context.SaveChangesAsync();
        return Results.Accepted($"Updated ID: {Id}");
    }
})
.WithName("UpdateSectorssummary")
.WithOpenApi();



        // POST create
        group.MapPost("/", async (Sectorssummary input) =>
        {
            using (var context = new EnterpriseContext())
            {
                context.Sectorssummaries.Add(input);
                await context.SaveChangesAsync();
                return TypedResults.Created("Created ID:" + input.Id);
            }
        })
        .WithName("CreateSectorssummary")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{Id}", async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["Id"] ?? httpContext.Request.RouteValues["id"];
            var Id = Convert.ToInt32(idObj);
            using (var context = new EnterpriseContext())
            {
                var summary = context.Sectorssummaries.FirstOrDefault(m => m.Id == Id);
                if (summary == null)
                    return Results.NotFound();

                context.Sectorssummaries.Remove(summary);
                await context.SaveChangesAsync();
                return Results.Ok();
            }
        })
        .WithName("DeleteSectorssummary")
        .WithOpenApi();
    }
}
