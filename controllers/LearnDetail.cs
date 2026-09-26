using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using Enterprise.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http;
namespace somecontrollers.Controllers;

public static class LearndetailEndpoints
{
    public static void MapLearndetailEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Learndetail").WithTags(nameof(Learndetail));

        // GET ALL
        group.MapGet("/", () =>
        {
            using var context = new EnterpriseContext();
            return Results.Ok(context.Learndetails.ToList());
        })
        .WithName("GetAllLearndetails")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{id}", (int id) =>
        {
            using var context = new EnterpriseContext();
            var item = context.Learndetails.FirstOrDefault(m => m.Id == id);

            return item is null
                ? Results.NotFound()
                : Results.Ok(item);
        })
        .WithName("GetLearndetailById")
        .WithOpenApi();

        // UPDATE
        group.MapPut("/{id}", async (int id, Learndetail input) =>
        {
            using var context = new EnterpriseContext();
            var item = context.Learndetails.FirstOrDefault(m => m.Id == id);

            if (item is null)
                return Results.NotFound();

            item.Description = input.Description;
            item.Userid = input.Userid;
            item.Employeeidasint = input.Employeeidasint;
            item.Employee = input.Employee;
            item.Employeeid = input.Employeeid;
            item.Learningmodulesid = input.Learningmodulesid;
            item.Cataloguesku = input.Cataloguesku;

            await context.SaveChangesAsync();
            return Results.Ok(item);
        })
        .WithName("UpdateLearndetail")
        .WithOpenApi();

        // CREATE
        group.MapPost("/", async (Learndetail input) =>
        {
            using var context = new EnterpriseContext();
            context.Learndetails.Add(input);
            await context.SaveChangesAsync();

            return Results.Created($"/api/Learndetail/{input.Id}", input);
        })
        .WithName("CreateLearndetail")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (int id) =>
        {
            using var context = new EnterpriseContext();
            var item = context.Learndetails.FirstOrDefault(m => m.Id == id);

            if (item is null)
                return Results.NotFound();

            context.Learndetails.Remove(item);
            await context.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("DeleteLearndetail")
        .WithOpenApi();
    }
}
