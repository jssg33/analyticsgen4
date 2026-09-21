using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Customers").WithTags(nameof(Customer));

        // GET ALL
        group.MapGet("/", () =>
        {
            using var context = new EnterpriseContext();
            return context.Customers.ToList();
        })
        .WithName("GetAllCustomers")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{id}", (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var id = Convert.ToInt32(idObj);
            using var context = new EnterpriseContext();
            return Results.Ok(context.Customers.Where(c => c.Id == id).ToList());
        })
        .WithName("GetCustomerById")
        .WithOpenApi();

        // CREATE
        group.MapPost("/", async (Customer input) =>
        {
            using var context = new EnterpriseContext();
            context.Customers.Add(input);
            await context.SaveChangesAsync();
            return TypedResults.Created("Created ID:" + input.Id);
        })
        .WithName("CreateCustomer")
        .WithOpenApi();

        // UPDATE
        group.MapPut("/{id}", async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var id = Convert.ToInt32(idObj);
            var input = await httpContext.Request.ReadFromJsonAsync<Customer>();
            using var context = new EnterpriseContext();
            var entity = context.Customers.Where(c => c.Id == id).FirstOrDefault();
            if (entity == null) return Results.NotFound();

            context.Customers.Attach(entity);
            entity.FullName = input!.FullName;
            entity.UserId = input.UserId;

            await context.SaveChangesAsync();
            return Results.Accepted($"Updated ID:{id}");
        })
        .WithName("UpdateCustomer")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"] ?? httpContext.Request.RouteValues["Id"];
            var id = Convert.ToInt32(idObj);
            using var context = new EnterpriseContext();
            var entity = context.Customers.Where(c => c.Id == id).FirstOrDefault();
            if (entity == null) return Results.NotFound();

            context.Customers.Attach(entity);
            context.Customers.Remove(entity);
            await context.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("DeleteCustomer")
        .WithOpenApi();
    }
}
