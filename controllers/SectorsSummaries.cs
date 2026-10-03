using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class SectorssummaryEndpoints
{
    private static bool ValidateToken(string token)
    {
        // Replace with your actual security validation
        return !string.IsNullOrWhiteSpace(token)
               && token == "EnterpriseSecurityToken";
    }

    public static void MapSectorssummaryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Sectorsummaries")
            .WithTags(nameof(Sectorssummary));

        // =====================================================
        // INFORMATION ENDPOINT
        // =====================================================

        group.MapGet("/", () =>
        {
            return Results.Ok(new
            {
                Message = "This endpoint requires a security token.",
                Controller = "Sectorssummary",
                Usage = "/api/Sectorsummaries/token/{token}",
                Security = "TokenRequired"
            });
        })
        .WithName("SectorssummaryInfo")
        .WithOpenApi();

        // =====================================================
        // TOKEN VALIDATION
        // =====================================================

        group.MapGet("/validate/{token}", (string token) =>
        {
            return Results.Ok(new
            {
                Valid = ValidateToken(token),
                Message = ValidateToken(token)
                    ? "Token accepted."
                    : "Token validation failed."
            });
        })
        .WithName("ValidateSectorssummaryToken")
        .WithOpenApi();

        // =====================================================
        // HELP
        // =====================================================

        group.MapGet("/help/{token}", (string token) =>
        {
            if (!ValidateToken(token))
            {
                return Results.Ok(new
                {
                    Authorized = false,
                    Message = "Invalid security token."
                });
            }

            return Results.Ok(new
            {
                Controller = "Sectorssummary",
                Endpoints = new[]
                {
                    "GET /api/Sectorsummaries/token/{token}",
                    "GET /api/Sectorsummaries/token/{token}/{id}",
                    "POST /api/Sectorsummaries/token/{token}",
                    "PUT /api/Sectorsummaries/token/{token}/{id}",
                    "DELETE /api/Sectorsummaries/token/{token}/{id}",
                    "POST /api/Sectorsummaries/token/{token}/bulk"
                }
            });
        })
        .WithName("SectorssummaryHelp")
        .WithOpenApi();

        // =====================================================
        // GET ALL (SECURE)
        // =====================================================

        group.MapGet("/token/{token}", (string token) =>
        {
            if (!ValidateToken(token))
            {
                return Results.Ok(new
                {
                    Authorized = false,
                    Message = "Invalid or missing security token."
                });
            }

            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.Sectorssummaries.ToList());
            }
        })
        .WithName("GetAllSectorssummariesSecure")
        .WithOpenApi();

        // =====================================================
        // GET BY ID (SECURE)
        // =====================================================

        group.MapGet("/token/{token}/{Id}",
        (
            string token,
            int Id) =>
        {
            if (!ValidateToken(token))
            {
                return Results.Ok(new
                {
                    Authorized = false,
                    Message = "Invalid security token."
                });
            }

            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.Sectorssummaries
                        .Where(m => m.Id == Id)
                        .ToList());
            }
        })
        .WithName("GetSectorssummaryByIdSecure")
        .WithOpenApi();

        // =====================================================
        // UPDATE (SECURE)
        // =====================================================

        group.MapPut("/token/{token}/{Id}",
        async (
            string token,
            HttpContext httpContext) =>
        {
            if (!ValidateToken(token))
            {
                return Results.Ok(new
                {
                    Authorized = false,
                    Message = "Invalid security token."
                });
            }

            var idObj =
                httpContext.Request.RouteValues["Id"] ??
                httpContext.Request.RouteValues["id"];

            var Id = Convert.ToInt32(idObj);

            var input =
                await httpContext.Request
                    .ReadFromJsonAsync<Sectorssummary>();

            if (input == null)
                return Results.BadRequest(
                    "Invalid Sectorssummary payload");

            using (var context = new EnterpriseContext())
            {
                var summary =
                    context.Sectorssummaries
                        .FirstOrDefault(m => m.Id == Id);

                if (summary == null)
                    return Results.NotFound();

                context.Sectorssummaries.Attach(summary);

                void UpdateIfNotNull<T>(
                    Action<T> setter,
                    object? value)
                {
                    if (value is T v)
                        setter(v);
                }

                // STRING FIELDS

                UpdateIfNotNull(
                    (string v) => summary.Company = v,
                    input.Company);

                UpdateIfNotNull(
                    (string v) => summary.Ticker = v,
                    input.Ticker);

                UpdateIfNotNull(
                    (string v) => summary.Sector = v,
                    input.Sector);

                UpdateIfNotNull(
                    (string v) => summary.Selected = v,
                    input.Selected);

                // DOUBLE? FIELDS

                UpdateIfNotNull(
                    (double? v) => summary.Avgdividend = v,
                    input.Avgdividend);

                UpdateIfNotNull(
                    (double? v) => summary.Totaldividends = v,
                    input.Totaldividends);

                UpdateIfNotNull(
                    (double? v) => summary.PriceStart = v,
                    input.PriceStart);

                UpdateIfNotNull(
                    (double? v) => summary.PriceEnd = v,
                    input.PriceEnd);

                UpdateIfNotNull(
                    (double? v) => summary.Change = v,
                    input.Change);

                UpdateIfNotNull(
                    (double? v) => summary.Totalreturn = v,
                    input.Totalreturn);

                UpdateIfNotNull(
                    (double? v) => summary.Totalreturnover10 = v,
                    input.Totalreturnover10);

                UpdateIfNotNull(
                    (double? v) => summary.Shares500 = v,
                    input.Shares500);

                UpdateIfNotNull(
                    (double? v) => summary.Totalspend = v,
                    input.Totalspend);

                UpdateIfNotNull(
                    (double? v) => summary.Fiveyearequityproj = v,
                    input.Fiveyearequityproj);

                UpdateIfNotNull(
                    (double? v) => summary.Fiveyeardivproj = v,
                    input.Fiveyeardivproj);

                UpdateIfNotNull(
                    (double? v) => summary.Totalfiveyearview = v,
                    input.Totalfiveyearview);

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"/api/Sectorsummaries/{Id}",
                    new
                    {
                        Message = "Updated",
                        Id = Id
                    });
            }
        })
        .WithName("UpdateSectorssummarySecure")
        .WithOpenApi();

        // =====================================================
        // CREATE (SECURE)
        // =====================================================

        group.MapPost("/token/{token}",
        async (
            string token,
            Sectorssummary input) =>
        {
            if (!ValidateToken(token))
            {
                return Results.Ok(new
                {
                    Authorized = false,
                    Message = "Invalid security token."
                });
            }

            using (var context = new EnterpriseContext())
            {
                input.Id = 0;

                context.Sectorssummaries.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/Sectorsummaries/{input.Id}",
                    new
                    {
                        Message = "Created",
                        Id = input.Id
                    });
            }
        })
        .WithName("CreateSectorssummarySecure")
        .WithOpenApi();

        // =====================================================
        // DELETE (SECURE)
        // =====================================================

        group.MapDelete("/token/{token}/{Id}",
        async (
            string token,
            int Id) =>
        {
            if (!ValidateToken(token))
            {
                return Results.Ok(new
                {
                    Authorized = false,
                    Message = "Invalid security token."
                });
            }

            using (var context = new EnterpriseContext())
            {
                var summary =
                    context.Sectorssummaries
                        .FirstOrDefault(m => m.Id == Id);

                if (summary == null)
                    return Results.NotFound();

                context.Sectorssummaries.Remove(summary);

                await context.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = "Deleted",
                    Id = Id
                });
            }
        })
        .WithName("DeleteSectorssummarySecure")
        .WithOpenApi();

        // =====================================================
        // BULK INSERT (SECURE)
        // =====================================================

        group.MapPost("/token/{token}/bulk",
        async (
            string token,
            List<Sectorssummary> inputs) =>
        {
            if (!ValidateToken(token))
            {
                return Results.Ok(new
                {
                    Authorized = false,
                    Message = "Invalid security token."
                });
            }

            if (inputs == null || !inputs.Any())
            {
                return Results.BadRequest(
                    "No records supplied.");
            }

            using (var context = new EnterpriseContext())
            {
                foreach (var item in inputs)
                {
                    item.Id = 0;
                }

                await context.Sectorssummaries
                    .AddRangeAsync(inputs);

                await context.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = $"{inputs.Count} sector summaries inserted.",
                    Count = inputs.Count
                });
            }
        })
        .WithName("CreateSectorssummariesBulkSecure")
        .WithOpenApi();
    }
}
