using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Enterprise.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;

namespace somecontrollers.Controllers;

public static class CockyCipherBlockEndpoints
{
    public static void MapCockyCipherBlockEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/CockyCipherBlock")
            .WithTags(nameof(CockyCipherBlock));

        // Get All
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.CockyCipherBlocks.ToList();
            }
        })
        .WithName("GetAllCockyCipherBlocks")
        .WithOpenApi();

        // Get By Id
        group.MapGet("/{id}", (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.CockyCipherBlocks
                    .Where(m => m.Id == id)
                    .ToList();
            }
        })
        .WithName("GetCockyCipherBlockById")
        .WithOpenApi();

        // Update
        group.MapPut("/{id}", async (int id, CockyCipherBlock input) =>
        {
            using (var context = new EnterpriseContext())
            {
                CockyCipherBlock[] blocks = context.CockyCipherBlocks
                    .Where(m => m.Id == id)
                    .ToArray();

                context.CockyCipherBlocks.Attach(blocks[0]);

                blocks[0].Description = input.Description;
                blocks[0].Key1 = input.Key1;
                blocks[0].Key2 = input.Key2;
                blocks[0].Key3 = input.Key3;
                blocks[0].Key4 = input.Key4;
                blocks[0].Key5 = input.Key5;
                blocks[0].type = input.type;
                blocks[0].SessionToken = input.SessionToken;

                await context.SaveChangesAsync();

                return TypedResults.Accepted("Updated ID:" + input.Id);
            }
        })
        .WithName("UpdateCockyCipherBlock")
        .WithOpenApi();

        // Create
        group.MapPost("/", async (CockyCipherBlock input) =>
        {
            using (var context = new EnterpriseContext())
            {
                context.CockyCipherBlocks.Add(input);

                await context.SaveChangesAsync();

                return TypedResults.Created("Created ID:" + input.Id);
            }
        })
        .WithName("CreateCockyCipherBlock")
        .WithOpenApi();

        // Delete
        group.MapDelete("/{id}", async (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                CockyCipherBlock[] blocks = context.CockyCipherBlocks
                    .Where(m => m.Id == id)
                    .ToArray();

                context.CockyCipherBlocks.Attach(blocks[0]);
                context.CockyCipherBlocks.Remove(blocks[0]);

                await context.SaveChangesAsync();
            }
        })
        .WithName("DeleteCockyCipherBlock")
        .WithOpenApi();
    }
}
