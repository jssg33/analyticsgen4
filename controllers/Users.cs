using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using Enterprise.Models;
namespace somecontrollers.Controllers;

public static class UserEndpoints
{

    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Users").WithTags(nameof(User));

        //[HttpGet]
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Users.ToList();
            }

        })
        .WithName("GetAllUsers")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/{id}", (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Users.Where(m => m.Id == id).ToList();
            }
        })
        .WithName("GetUserById")
        .WithOpenApi();

     group.MapPut("/{id}", async (int id, User input) =>
{
    using (var context = new EnterpriseContext())
    {
        var existing = context.Users.FirstOrDefault(m => m.Id == id);

        if (existing == null)
        {
            return Results.NotFound("No record found for ID:" + id) as IResult;
        }

        if (input.Lastname != null) existing.Lastname = input.Lastname;
        if (input.Firstname != null) existing.Firstname = input.Firstname;
        if (input.Username != null) existing.Username = input.Username;
        if (input.Email != null) existing.Email = input.Email;
        if (input.Employee != null) existing.Employee = input.Employee;
        if (input.Employeeid != null) existing.Employeeid = input.Employeeid;
        if (input.Microsoftid != null) existing.Microsoftid = input.Microsoftid;
        if (input.Ncrid != null) existing.Ncrid = input.Ncrid;
        if (input.Oracleid != null) existing.Oracleid = input.Oracleid;
        if (input.Azureid != null) existing.Azureid = input.Azureid;
        if (input.Plainpassword != null) existing.Plainpassword = input.Plainpassword;
        if (input.Passwordtype != null) existing.Passwordtype = input.Passwordtype;
        if (input.Jid != null) existing.Jid = input.Jid;
        if (input.Profileurl != null) existing.Profileurl = input.Profileurl;
        if (input.Role != null) existing.Role = input.Role;
        if (input.Role != null) existing.Role2 = input.Role2;
        if (input.Role != null) existing.Role3 = input.Role3;
        if (input.Fullname != null) existing.Fullname = input.Fullname;
        await context.SaveChangesAsync();

        return TypedResults.Accepted("Updated ID:" + existing.Id) as IResult;
    }
})
.WithName("UpdateUser")
.WithOpenApi();

        group.MapPost("/", async (User input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Random rnd = new Random();
                int dice = rnd.Next(1000, 10000000);
                input.Id = dice;
                context.Users.Add(input);
                await context.SaveChangesAsync();
                return TypedResults.Created("Created ID:" + input.Id);
            }

        })
        .WithName("CreateUser")
        .WithOpenApi();

        group.MapPut("/logout/{token}", async (int id, string token, Usersession input) =>
        {
            using (var context = new EnterpriseContext())
            {
                Usersession[] someUsersession = context.Usersessions.Where(m => m.Token == token || m.Userid == id).ToArray();
                context.Usersessions.Attach(someUsersession[0]);
                DateTime utcNow = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, easternZone);
                //Console.WriteLine("Current time in EST: " + easternTime);
                input.Sessionend = easternTime.ToString();
                someUsersession[0].Sessionend = input.Sessionend;
                someUsersession[0].Sessioncomplete = 1;
                await context.SaveChangesAsync();
                return TypedResults.Accepted("Updated ID:" + input.Id);
            }


        })
        .WithName("LogoutUser")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using (var context = new EnterpriseContext())
            {
                //context.Users.Add(std);
                User[] someUsers = context.Users.Where(m => m.Id == id).ToArray();
                context.Users.Attach(someUsers[0]);
                context.Users.Remove(someUsers[0]);
                await context.SaveChangesAsync();
            }

        })
        .WithName("DeleteUser")
        .WithOpenApi();

        group.MapGet("/userjoin", () =>
        {
            using (var context = new EnterpriseContext())
            {
                var result = context.Users
                    .Join(
                        context.Userprofiles,
                        user => user.Id,
                        profile => profile.Userid,
                        (user, profile) => new
                        {
                            UserId = user.Id,
                            Username = user.Username,
                            Fullname = user.Fullname,
                            Email = user.Email,
                            Role = user.Role,
                            ProfileUrl = user.Profileurl,
                            
                            // UserProfile data
                            Address1 = profile.Address1,
                            Address2 = profile.Address2,
                            City = profile.City,
                            StateRegion = profile.Stateregion,
                            Country = profile.Country,
                            Phone = profile.Phone,
                            Cellphone = profile.Cellphone,
                            MaritalStatus = profile.Maritalstatus,
                            University1 = profile.University1,
                            LinkedinUrl = profile.Linkedinurl,
                            InstagramUrl = profile.Instagramurl,
                            FacebookUrl = profile.Facebookurl,
                            GoogleUrl = profile.Googleurl,
                            Title = profile.Title,
                            Pronoun = profile.Pronoun,
                            ActivePictureUrl = profile.Activepictureurl,
                            PostalZip = profile.Postalzip
                        }
                    )
                    .ToList();

                return result;
            }
        })
        .WithName("GetUserJoin")
        .WithOpenApi();

    }
}

