using Enterprise.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace somecontrollers.Controllers;

public static class ApilogEndpoints
{
    public static void MapApilogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Apilog").WithTags(nameof(Apilog));

        group.MapGet("/", async () =>
        {
            using var context = new EnterpriseContext();
            return await context.Apilogs.AsNoTracking().ToListAsync();
        })
        .WithName("GetAllApilogs")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Apilog>, NotFound>> (int id) =>
        {
            using var context = new EnterpriseContext();
            var apiLog = await context.Apilogs.AsNoTracking()
                .FirstOrDefaultAsync(log => log.Id == id);

            return apiLog is null ? TypedResults.NotFound() : TypedResults.Ok(apiLog);
        })
        .WithName("GetApilogById")
        .WithOpenApi();

        group.MapPost("/", async Task<Created<Apilog>> (Apilog input) =>
        {
            using var context = new EnterpriseContext();
            context.Apilogs.Add(input);
            await context.SaveChangesAsync();
            return TypedResults.Created($"/api/Apilog/{input.Id}", input);
        })
        .WithName("CreateApilog")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<NoContent, NotFound>> (int id, Apilog input) =>
        {
            using var context = new EnterpriseContext();
            var apiLog = await context.Apilogs.FindAsync(id);
            if (apiLog is null)
            {
                return TypedResults.NotFound();
            }

            apiLog.Apiname = input.Apiname;
            apiLog.Apinumber = input.Apinumber;
            apiLog.Eptype = input.Eptype;
            apiLog.Hashid = input.Hashid;
            apiLog.Parameterlist = input.Parameterlist;
            apiLog.Apiresult = input.Apiresult;
            apiLog.Description = input.Description;
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        })
        .WithName("UpdateApilog")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<NoContent, NotFound>> (int id) =>
        {
            using var context = new EnterpriseContext();
            var apiLog = await context.Apilogs.FindAsync(id);
            if (apiLog is null)
            {
                return TypedResults.NotFound();
            }

            context.Apilogs.Remove(apiLog);
            await context.SaveChangesAsync();
            return TypedResults.NoContent();
        })
        .WithName("DeleteApilog")
        .WithOpenApi();
    }
}