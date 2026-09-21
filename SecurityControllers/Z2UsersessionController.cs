using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Enterprise.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace somecontrollers.Controllers;

public static class UsersessionEndpoints
{
    public static void MapUsersessionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Usersession").WithTags(nameof(Usersession));
        EnterpriseServices.Globals.ControllerAPIName = "UsersessionAPI";
        EnterpriseServices.Globals.ControllerAPINumber = "001";

        group.MapGet("/", () =>
        {
            using var context = new EnterpriseContext();
            EnterpriseServices.ApiLogger.logapi(
                EnterpriseServices.Globals.ControllerAPIName,
                EnterpriseServices.Globals.ControllerAPINumber,
                "GET", 1, "Test", "Test");
            return context.Usersessions.ToList();
        })
        .WithName("GetAllUsersessions")
        .WithOpenApi();

        group.MapGet("/{id}", (int id) =>
        {
            using var context = new EnterpriseContext();
            EnterpriseServices.ApiLogger.logapi(
                EnterpriseServices.Globals.ControllerAPIName,
                EnterpriseServices.Globals.ControllerAPINumber,
                "GETWITHID", 1, "Test", "Test");
            return context.Usersessions.Where(session => session.Id == id).ToList();
        })
        .WithName("GetUsersessionById")
        .WithOpenApi();

        group.MapGet("/user/{userId}", (string userId) =>
        {
            using var context = new EnterpriseContext();
            EnterpriseServices.ApiLogger.logapi(
                EnterpriseServices.Globals.ControllerAPIName,
                EnterpriseServices.Globals.ControllerAPINumber,
                "GETWITHUSERID", 1, "Test", "Test");
            return context.Usersessions
                .Where(session => session.Useridasstring == userId)
                .ToList();
        })
        .WithName("GetUsersessionByUserId")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, Usersession input) =>
        {
            using var context = new EnterpriseContext();
            var session = context.Usersessions.FirstOrDefault(item => item.Id == id);
            if (session is null)
            {
                return Results.NotFound();
            }

            if (input.Sessiondescription is not null)
            {
                session.Sessiondescription = input.Sessiondescription;
            }

            await context.SaveChangesAsync();
            EnterpriseServices.ApiLogger.logapi(
                EnterpriseServices.Globals.ControllerAPIName,
                EnterpriseServices.Globals.ControllerAPINumber,
                "PUTWITHID", 1, "Test", "Test");
            return TypedResults.Accepted($"Updated ID:{id}");
        })
        .WithName("UpdateUsersession")
        .WithOpenApi();

        group.MapPost("/{userId:int}", async (int userId) =>
        {
            using var context = new EnterpriseContext();
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var user = await context.Users.FirstOrDefaultAsync(item => item.Id == userId);
            if (user is null)
            {
                return Results.BadRequest("User not found.");
            }

            var jwtKey = config["Jwt:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("role", user.Role ?? string.Empty)
            };

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: signingCredentials));

            var newSession = new Usersession
            {
                Userid = user.Id,
                Useridasstring = user.Id.ToString(),
                Token = token,
                Sessionstart = DateTime.UtcNow.ToString("o"),
                Sessiondescription = "Session created",
                Sessionusername = user.Username,
                Sessionemail = user.Email,
                Sessionfirstname = user.Firstname,
                Sessionlastname = user.Lastname,
                Sessionfullname = user.Fullname,
                Sessioncomplete = 0,
                Acknowledged = 0
            };

            context.Usersessions.Add(newSession);
            await context.SaveChangesAsync();
            EnterpriseServices.ApiLogger.logapi(
                EnterpriseServices.Globals.ControllerAPIName,
                EnterpriseServices.Globals.ControllerAPINumber,
                "NEWSESSION", 1, "TEST", "TEST");

            return TypedResults.Created($"/api/Usersession/{newSession.Id}", new
            {
                newSession.Id,
                newSession.Userid,
                newSession.Token,
                newSession.Sessionstart
            });
        })
        .WithName("CreateUsersessionForUser")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using var context = new EnterpriseContext();
            var session = context.Usersessions.FirstOrDefault(item => item.Id == id);
            if (session is null)
            {
                return Results.NotFound();
            }

            context.Usersessions.Remove(session);
            await context.SaveChangesAsync();
            EnterpriseServices.ApiLogger.logapi(
                EnterpriseServices.Globals.ControllerAPIName,
                EnterpriseServices.Globals.ControllerAPINumber,
                "DELETEWITHID", 1, "TEST", "TEST");
            return Results.NoContent();
        })
        .WithName("DeleteUsersession")
        .WithOpenApi();
    }
}