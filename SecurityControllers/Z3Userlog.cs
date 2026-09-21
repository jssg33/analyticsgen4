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

public static class UserlogEndpoints
{

    public static void MapUserlogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Userlog").WithTags(nameof(Userlog));

        //[HttpGet]
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Userlogs.ToList();
            }

        })
        .WithName("V83GetAllUserlogs")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/{id}", (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Userlogs.Where(m => m.Id == id).ToList();
            }
        })
        .WithName("GetUserlogById")
        .WithOpenApi();

        //[HttpPut]
        group.MapPut("/{id}", async (int id, Userlog input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Userlog[] someUserlog = context.Userlogs.Where(m => m.Id == id).ToArray();
                context.Userlogs.Attach(someUserlog[0]);
                someUserlog[0].Description = input.Description;
                someUserlog[0].uid = input.uid;
                someUserlog[0].role = input.role;                 
                await context.SaveChangesAsync();
                return TypedResults.Accepted("Updated ID:" + input.Id);
            }


        })
        .WithName("UpdateUserlog")
        .WithOpenApi();

        group.MapPost("/", async (Userlog input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Random rnd = new Random();
                int dice = rnd.Next(1000, 10000000);
                //input.Id = dice;
                context.Userlogs.Add(input);
                await context.SaveChangesAsync();
                return TypedResults.Created("Created ID:" + input.Id);
            }

        })
        .WithName("CreateUserlog")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                //context.Userlogs.Add(std);
                Userlog[] someUserlogs = context.Userlogs.Where(m => m.Id == id).ToArray();
                context.Userlogs.Attach(someUserlogs[0]);
                context.Userlogs.Remove(someUserlogs[0]);
                await context.SaveChangesAsync();
            }

        })
        .WithName("DeleteUserlog")
        .WithOpenApi();
    }
}

