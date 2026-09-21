using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Enterprise.Models;
using EnterpriseServices;
using Microsoft.Extensions.WebEncoders.Testing;
namespace Enterprise.Controllers;


public static class SuperuserlogEndpoints
{
    
    public static void MapSuperuserlogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Superuserlog").WithTags(nameof(Superuserlog));
        EnterpriseServices.Globals.ControllerAPIName = "SuperuserlogAPI";
        EnterpriseServices.Globals.ControllerAPINumber = "001";
        
        //[HttpGet]
        group.MapGet("/", () =>
        {
           

            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET", 1, "Test", "Test");
                return context.Superuserlogs.ToList();
            }
            
        })
        .WithName("GetAllSuperuserlogs")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/{id}", (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GETWITHID", 1, "Test", "Test"); 
                return context.Superuserlogs.Where(m => m.Id == id).ToList();
            }
        })
        .WithName("GetSuperuserlogById")
        .WithOpenApi();

        //[HttpPut]
        group.MapPut("/{id}", async (int id, Superuserlog input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Superuserlog[] someSuperuserlog = context.Superuserlogs.Where(m => m.Id == id).ToArray();
                context.Superuserlogs.Attach(someSuperuserlog[0]);
                if (input.Description != null) someSuperuserlog[0].Description = input.Description;
                await context.SaveChangesAsync();
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "PUTWITHID", 1, "Test", "Test");
                return TypedResults.Accepted("Updated ID:" + input.Id);
            }


        })
        .WithName("UpdateSuperuserlog")
        .WithOpenApi();

        group.MapPost("/", async (Superuserlog input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Random rnd = new Random();
                int dice = rnd.Next(1000, 10000000);
                //input.Id = dice;
                context.Superuserlogs.Add(input);
                await context.SaveChangesAsync();
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "NEWRECORD", 1, "TEST", "TEST");
                return TypedResults.Created("Created ID:" + input.Id);
            }

        })
        .WithName("CreateSuperuserlog")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                //context.Superuserlogs.Add(std);
                Superuserlog[] someSuperuserlogs = context.Superuserlogs.Where(m => m.Id == id).ToArray();
                context.Superuserlogs.Attach(someSuperuserlogs[0]);
                context.Superuserlogs.Remove(someSuperuserlogs[0]);
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "DELETEWITHID",1, "TEST", "TEST");
                await context.SaveChangesAsync();
            }

        })
        .WithName("DeleteSuperuserlog")
        .WithOpenApi();
    }
}

