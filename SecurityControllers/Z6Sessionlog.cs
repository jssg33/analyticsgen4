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
using Microsoft.Extensions.WebEncoders.Testing;
namespace Enterprise.Controllers;


public static class SessionlogEndpoints
{
    
    public static void MapSessionlogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Sessionlog").WithTags(nameof(Sessionlog));
        EnterpriseServices.Globals.ControllerAPIName = "SessionlogAPI";
        EnterpriseServices.Globals.ControllerAPINumber = "001";
        
        //[HttpGet]
        group.MapGet("/", () =>
        {
           

            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET", 1, "Test", "Test");
                return context.Sessionlogs.ToList();
            }
            
        })
        .WithName("GetAllSessionlogs")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/{id}", (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GETWITHID", 1, "Test", "Test"); 
                return context.Sessionlogs.Where(m => m.Id == id).ToList();
            }
        })
        .WithName("GetSessionlogById")
        .WithOpenApi();

        //[HttpPut]
        group.MapPut("/{id}", async (int id, Sessionlog input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Sessionlog[] someSessionlog = context.Sessionlogs.Where(m => m.Id == id).ToArray();
                context.Sessionlogs.Attach(someSessionlog[0]);
                if (input.Description != null) someSessionlog[0].Description = input.Description;
                await context.SaveChangesAsync();
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "PUTWITHID", 1, "Test", "Test");
                return TypedResults.Accepted("Updated ID:" + input.Id);
            }


        })
        .WithName("UpdateSessionlog")
        .WithOpenApi();

        group.MapPost("/", async (Sessionlog input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Random rnd = new Random();
                int dice = rnd.Next(1000, 10000000);
                //input.Id = dice;
                context.Sessionlogs.Add(input);
                await context.SaveChangesAsync();
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "NEWRECORD", 1, "TEST", "TEST");
                return TypedResults.Created("Created ID:" + input.Id);
            }

        })
        .WithName("CreateSessionlog")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                //context.Sessionlogs.Add(std);
                Sessionlog[] someSessionlogs = context.Sessionlogs.Where(m => m.Id == id).ToArray();
                context.Sessionlogs.Attach(someSessionlogs[0]);
                context.Sessionlogs.Remove(someSessionlogs[0]);
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "DELETEWITHID",1, "TEST", "TEST");
                await context.SaveChangesAsync();
            }

        })
        .WithName("DeleteSessionlog")
        .WithOpenApi();
    }
}

