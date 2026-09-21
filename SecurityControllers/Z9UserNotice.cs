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
namespace somecontrollers.Controllers;


public static class UsernoticeEndpoints
{
    
    public static void MapUserNoticeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Usernotices").WithTags(nameof(Usernotice));
        EnterpriseServices.Globals.ControllerAPIName = "UsernoticeAPI";
        EnterpriseServices.Globals.ControllerAPINumber = "001";
        
        //[HttpGet]
        group.MapGet("/", () =>
        {
           

            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET", 1, "Test", "Test");
                return context.Usernotices.ToList();
            }
            
        })
        .WithName("GetAllUsernotices")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/{id}", (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GETWITHID", 1, "Test", "Test"); 
                return context.Usernotices.Where(m => m.Id == id).ToList();
            }
        })
        .WithName("GetUsernoticeById")
        .WithOpenApi();

        //[HttpPut]
        group.MapPut("/{id}", async (int id, Usernotice input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Usernotice[] someUsernotice = context.Usernotices.Where(m => m.Id == id).ToArray();
                context.Usernotices.Attach(someUsernotice[0]);
                if (input.Description != null) someUsernotice[0].Description = input.Description;
                await context.SaveChangesAsync();
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "PUTWITHID", 1, "Test", "Test");
                return TypedResults.Accepted("Updated ID:" + input.Id);
            }


        })
        .WithName("UpdateUsernotice")
        .WithOpenApi();

        group.MapPost("/", async (Usernotice input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Random rnd = new Random();
                int dice = rnd.Next(1000, 10000000);
                //input.Id = dice;
                context.Usernotices.Add(input);
                await context.SaveChangesAsync();
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "NEWRECORD", 1, "TEST", "TEST");
                return TypedResults.Created("Created ID:" + input.Id);
            }

        })
        .WithName("CreateUsernotice")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                //context.Usernoticess.Add(std);
                Usernotice[] someUsernotices = context.Usernotices.Where(m => m.Id == id).ToArray();
                context.Usernotices.Attach(someUsernotices[0]);
                context.Usernotices.Remove(someUsernotices[0]);
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "DELETEWITHID",1, "TEST", "TEST");
                await context.SaveChangesAsync();
            }

        })
        .WithName("DeleteUsernotice")
        .WithOpenApi();
    }
}

