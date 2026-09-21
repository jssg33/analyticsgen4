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

public static class UserprofileEndpoints
{

    public static void MapUserprofileEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Userprofile").WithTags(nameof(Userprofile));

        //[HttpGet]
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Userprofiles.ToList();
            }

        })
        .WithName("GetAllUserprofiles")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/{id}", (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Userprofiles.Where(m => m.Id == id).ToList();
            }
        })
        .WithName("GetUserprofileById")
        .WithOpenApi();

        group.MapGet("/uid/{Userid}", (int Userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Userprofiles.Where(m => m.Userid == Userid).ToList();
            }
        })
   .WithName("GetUserprofileByUserId")
   .WithOpenApi();

//WE ARE MODIFYING LEGACY CODE TO CHECK IF THE USER RECORD EXISTS FIRST. IF IT DOES WE CHECK IF THAT USER HAS A PROFILE. IF IT DOES WE UPDATE IT FOR RECORDS WITH VALUES. IF THAT USER DOESNT HAVE A PROFILE WE CREATE ONE.
        
group.MapPut("/uid/{Userid}", async (int Userid, Userprofile input) =>
{
    using (var context = new EnterpriseContext())
    {
        var userExists = context.Users.Any(u => u.Id == Userid);

        if (!userExists)
        {
            return Results.NotFound("No user with that id") as IResult;
        }

        var existing = context.Userprofiles.FirstOrDefault(m => m.Userid == Userid);

        bool isNew = existing == null;

        if (isNew)
        {
            existing = new Userprofile { Userid = Userid };
            context.Userprofiles.Add(existing);
        }

        if (input.Activepictureurl != null) existing!.Activepictureurl = input.Activepictureurl;
        if (input.Address1 != null) existing!.Address1 = input.Address1;
        if (input.Address2 != null) existing!.Address2 = input.Address2;
        if (input.Branchid != null) existing!.Branchid = input.Branchid;
        if (input.CipherSupportId != null) existing!.CipherSupportId = input.CipherSupportId;
        if (input.Cellphone != null) existing!.Cellphone = input.Cellphone;
        if (input.City != null) existing!.City = input.City;
        if (input.Companyid != null) existing!.Companyid = input.Companyid;
        if (input.Country != null) existing!.Country = input.Country;
        if (input.Defaultinstanceid != null) existing!.Defaultinstanceid = input.Defaultinstanceid;
        if (input.Defaultshardid != null) existing!.Defaultshardid = input.Defaultshardid;
        if (input.Email != null) existing!.Email = input.Email;
        if (input.Employeeid != null) existing!.Employeeid = input.Employeeid;
        if (input.Facebookurl != null) existing!.Facebookurl = input.Facebookurl;
        if (input.Firstname != null) existing!.Firstname = input.Firstname;
        if (input.Fullname != null) existing!.Fullname = input.Fullname;
        if (input.Googleurl != null) existing!.Googleurl = input.Googleurl;
        if (input.Instagramurl != null) existing!.Instagramurl = input.Instagramurl;
        if (input.Lastname != null) existing!.Lastname = input.Lastname;
        if (input.Linkedinurl != null) existing!.Linkedinurl = input.Linkedinurl;
        if (input.Managerid != null) existing!.Managerid = input.Managerid;
        if (input.Maritalstatus != null) existing!.Maritalstatus = input.Maritalstatus;
        if (input.Phone != null) existing!.Phone = input.Phone;
        if (input.Postalzip != null) existing!.Postalzip = input.Postalzip;
        if (input.Pronoun != null) existing!.Pronoun = input.Pronoun;
        if (input.Regionid != null) existing!.Regionid = input.Regionid;
        if (input.Sms != null) existing!.Sms = input.Sms;
        if (input.Stateregion != null) existing!.Stateregion = input.Stateregion;
        if (input.Title != null) existing!.Title = input.Title;
        if (input.Title2 != null) existing!.Title2 = input.Title2;
        if (input.University != null) existing!.University = input.University;
        if (input.University1 != null) existing!.University1 = input.University1;
        if (input.University2 != null) existing!.University2 = input.University2;
        if (input.Vimeourl != null) existing!.Vimeourl = input.Vimeourl;

        await context.SaveChangesAsync();

        return isNew
            ? Results.Created($"/uid/{existing!.Userid}", existing!)
            : TypedResults.Accepted("Updated ID:" + existing!.Id) as IResult;
    }
})
.WithName("UpdateUserprofileByUserId")
.WithOpenApi();
        
       
        group.MapPost("/", async (Userprofile input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Random rnd = new Random();
                int dice = rnd.Next(1000, 10000000);
                //input.Id = dice;
                context.Userprofiles.Add(input);
                await context.SaveChangesAsync();
                return TypedResults.Created("Created ID:" + input.Id);
            }

        })
        .WithName("CreateUserprofile")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                //context.Userprofiles.Add(std);
                Userprofile[] someUserprofiles = context.Userprofiles.Where(m => m.Id == id).ToArray();
                context.Userprofiles.Attach(someUserprofiles[0]);
                context.Userprofiles.Remove(someUserprofiles[0]);
                await context.SaveChangesAsync();
            }

        })
        .WithName("DeleteUserprofile")
        .WithOpenApi();
    }
}

