using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class AllstockEndpoints
{
    private static bool ValidateToken(string token)
    {
        // Replace with your actual auth logic
        return !string.IsNullOrWhiteSpace(token)
               && token == "EnterpriseSecurityToken";
    }

    public static void MapAllstockEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Allstock")
            .WithTags(nameof(Allstock));

        // =====================================================
        // INFORMATION ENDPOINT
        // =====================================================

        group.MapGet("/", () =>
        {
            return Results.Ok(new
            {
                Message = "This controller requires a security token.",
                Controller = "Allstock",
                Usage = "/api/Allstock/token/{token}",
                Security = "TokenRequired"
            });
        })
        .WithName("AllstockInfo")
        .WithOpenApi();

        // =====================================================
        // SECURE GET ALL
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
                return Results.Ok(context.Allstocks.ToList());
            }
        })
        .WithName("GetAllAllstocksSecure")
        .WithOpenApi();

        // =====================================================
        // SECURE GET BY ID
        // =====================================================

        group.MapGet("/token/{token}/{Id}", (
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
                    context.Allstocks
                        .Where(m => m.Id == Id)
                        .ToList());
            }
        })
        .WithName("GetAllstockByIdSecure")
        .WithOpenApi();

        // =====================================================
        // SECURE PUT
        // =====================================================

        group.MapPut("/token/{token}/{id}",
        async (
            string token,
            int id,
            Allstock input) =>
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
                var stock = context.Allstocks
                    .FirstOrDefault(m => m.Id == id);

                if (stock == null)
                    return Results.NotFound();

                context.Allstocks.Attach(stock);

                stock.Company = input.Company;

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"/api/Allstock/{id}",
                    new
                    {
                        Message = "Updated",
                        Id = id
                    });
            }
        })
        .WithName("UpdateAllstockSecure")
        .WithOpenApi();

        // =====================================================
        // SECURE POST
        // =====================================================

        group.MapPost("/token/{token}",
        async (
            string token,
            Allstock input) =>
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

                context.Allstocks.Add(input);

                await context.SaveChangesAsync();

                return TypedResults.Created(
                    $"/api/Allstock/{input.Id}",
                    new
                    {
                        Message = "Created",
                        Id = input.Id
                    });
            }
        })
        .WithName("CreateAllstockSecure")
        .WithOpenApi();

        // =====================================================
        // SECURE DELETE
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
                var stock = context.Allstocks
                    .FirstOrDefault(m => m.Id == Id);

                if (stock == null)
                    return Results.NotFound();

                context.Allstocks.Remove(stock);

                await context.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = "Deleted",
                    Id = Id
                });
            }
        })
        .WithName("DeleteAllstockSecure")
        .WithOpenApi();

        // =====================================================
        // SECURE BULK INSERT
        // =====================================================

        group.MapPost("/token/{token}/bulk",
        async (
            string token,
            List<Allstock> inputs) =>
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
                return Results.BadRequest("No records supplied.");

            using (var context = new EnterpriseContext())
            {
                foreach (var item in inputs)
                {
                    item.Id = 0;
                }

                await context.Allstocks.AddRangeAsync(inputs);

                await context.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = $"{inputs.Count} stocks inserted.",
                    Count = inputs.Count
                });
            }
        })
        .WithName("BulkCreateAllstockSecure")
        .WithOpenApi();

        // =====================================================
        // TOKEN VALIDATION TEST
        // =====================================================

        group.MapGet("/validate/{token}",
        (string token) =>
        {
            if (!ValidateToken(token))
            {
                return Results.Ok(new
                {
                    Valid = false,
                    Message = "Token validation failed."
                });
            }

            return Results.Ok(new
            {
                Valid = true,
                Message = "Token accepted."
            });
        })
        .WithName("ValidateAllstockToken")
        .WithOpenApi();

        // =====================================================
        // CONTROLLER INFORMATION
        // =====================================================

        group.MapGet("/help/{token}",
        (string token) =>
        {
            if (!ValidateToken(token))
            {
                return Results.Ok(new
                {
                    Authorized = false,
                    Message = "Invalid token."
                });
            }

            return Results.Ok(new
            {
                Controller = "Allstock",
                Description = "Stock Management Controller",
                Endpoints = new[]
                {
                    "GET /api/Allstock/token/{token}",
                    "GET /api/Allstock/token/{token}/{id}",
                    "POST /api/Allstock/token/{token}",
                    "PUT /api/Allstock/token/{token}/{id}",
                    "DELETE /api/Allstock/token/{token}/{id}",
                    "POST /api/Allstock/token/{token}/bulk"
                }
            });
        })
        .WithName("AllstockHelp")
        .WithOpenApi();
    }
}
