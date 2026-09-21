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

public static class UserhelpEndpoints
{

    public static void MapUserhelpEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Userhelp").WithTags(nameof(Userhelp));

        //[HttpGet]
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Userhelps.ToList();
            }

        })
        .WithName("GetAllUserhelps")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/{id}", (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Userhelps.Where(m => m.Id == id).ToList();
            }
        })
        .WithName("GetUserhelpById")
        .WithOpenApi();

        //[HttpPut]
        group.MapPut("/{id}", async (int id, Userhelp input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Userhelp[] someUserhelp = context.Userhelps.Where(m => m.Id == id).ToArray();
                context.Userhelps.Attach(someUserhelp[0]);
                someUserhelp[0].Descr = input.Descr;
                someUserhelp[0].Emplid = input.Emplid;
                someUserhelp[0].Severity = input.Severity;
                await context.SaveChangesAsync();
                return TypedResults.Accepted("Updated ID:" + input.Id);
            }


        })
        .WithName("UpdateUserhelp")
        .WithOpenApi();

        group.MapPost("/", async (Userhelp input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Random rnd = new Random();
                int dice = rnd.Next(1000, 10000000);
                //input.Id = dice;
                context.Userhelps.Add(input);
                await context.SaveChangesAsync();
                return TypedResults.Created("Created ID:" + input.Id);
            }

        })
        .WithName("CreateUserhelp")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                //context.Userhelps.Add(std);
                Userhelp[] someUserhelps = context.Userhelps.Where(m => m.Id == id).ToArray();
                context.Userhelps.Attach(someUserhelps[0]);
                context.Userhelps.Remove(someUserhelps[0]);
                await context.SaveChangesAsync();
            }

        })
        .WithName("DeleteUserhelp")
        .WithOpenApi();
    }
}

