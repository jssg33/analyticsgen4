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
using EnterpriseServices;
namespace somecontrollers.Controllers;

public static class UserLocationEndpoints
{
    public static void MapUserLocationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/userlocation").WithTags("UserLocation");

        // GET by userid
        group.MapGet("/{userid}", async (int userid) =>
        {
            using var context = new EnterpriseContext();

            var userExists = context.Users.Any(u => u.Id == userid);
            if (!userExists)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "GET-USER-NOTFOUND", 1, "Test", "Test");
                return Results.NotFound("No user with that id") as IResult;
            }

            var locations = context.UserLocations.Where(l => l.UserId == userid).ToList();

            ApiLogger.logapi("UserLocationAPI", "024", "GETWITHUSERID", 1, "Test", "Test");
            return Results.Ok(locations) as IResult;
        });

        // GET by sessionid
        group.MapGet("/session/{sessionid}", async (int sessionid) =>
        {
            using var context = new EnterpriseContext();

            var session = context.Usersessions.FirstOrDefault(s => s.Id == sessionid);
            if (session == null)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "GET-SESSION-NOTFOUND", 1, "Test", "Test");
                return Results.NotFound("No session with that id") as IResult;
            }

            if (session.Userid == null)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "GET-SESSION-NOUSER", 1, "Test", "Test");
                return Results.NotFound("Session has no associated user") as IResult;
            }

            var locations = context.UserLocations.Where(l => l.UserId == session.Userid).ToList();

            ApiLogger.logapi("UserLocationAPI", "024", "GETWITHSESSIONID", 1, "Test", "Test");
            return Results.Ok(locations) as IResult;
        });

        // PUT by userid
        group.MapPut("/{userid}", async (int userid, UserLocation input) =>
        {
            using var context = new EnterpriseContext();

            var userExists = context.Users.Any(u => u.Id == userid);
            if (!userExists)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "PUT-USER-NOTFOUND", 1, "Test", "Test");
                return Results.NotFound("No user with that id") as IResult;
            }

            var existing = await context.UserLocations
                .FirstOrDefaultAsync(l => l.UserId == userid && l.Id == input.Id);

            if (existing == null)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "PUT-LOCATION-NOTFOUND", 1, "Test", "Test");
                return Results.NotFound($"No location {input.Id} found for user {userid}") as IResult;
            }

            // Update only allowed fields
            if (input.Label != null)
                existing.Label = input.Label;
            if (input.AddressLine1 != null)
                existing.AddressLine1 = input.AddressLine1;
            if (input.AddressLine2 != null)
                existing.AddressLine2 = input.AddressLine2;
            if (input.City != null)
                existing.City = input.City;
            if (input.State != null)
                existing.State = input.State;
            if (input.PostalCode != null)
                existing.PostalCode = input.PostalCode;
            if (input.Country != null)
                existing.Country = input.Country;
            if (input.Latitude != null)
                existing.Latitude = input.Latitude;
            if (input.Longitude != null)
                existing.Longitude = input.Longitude;
            if (input.IsPrimary != null)
                existing.IsPrimary = input.IsPrimary;

            await context.SaveChangesAsync();

            ApiLogger.logapi("UserLocationAPI", "024", "PUTWITHUSERID", 1, "Test", "Test");
            return Results.Accepted($"Updated ID: {existing.Id}") as IResult;
        });

        // PUT by sessionid
        group.MapPut("/session/{sessionid}", async (int sessionid, UserLocation input) =>
        {
            using var context = new EnterpriseContext();

            var session = context.Usersessions.FirstOrDefault(s => s.Id == sessionid);
            if (session == null)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "PUT-SESSION-NOTFOUND", 1, "Test", "Test");
                return Results.NotFound("No session with that id") as IResult;
            }

            if (session.Userid == null)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "PUT-SESSION-NOUSER", 1, "Test", "Test");
                return Results.NotFound("Session has no associated user") as IResult;
            }

            var existing = await context.UserLocations
                .FirstOrDefaultAsync(l => l.UserId == session.Userid && l.Id == input.Id);

            if (existing == null)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "PUT-LOCATION-NOTFOUND", 1, "Test", "Test");
                return Results.NotFound($"No location {input.Id} found for session {sessionid}") as IResult;
            }

            // Update only allowed fields
            if (input.Label != null)
                existing.Label = input.Label;
            if (input.AddressLine1 != null)
                existing.AddressLine1 = input.AddressLine1;
            if (input.AddressLine2 != null)
                existing.AddressLine2 = input.AddressLine2;
            if (input.City != null)
                existing.City = input.City;
            if (input.State != null)
                existing.State = input.State;
            if (input.PostalCode != null)
                existing.PostalCode = input.PostalCode;
            if (input.Country != null)
                existing.Country = input.Country;
            if (input.Latitude != null)
                existing.Latitude = input.Latitude;
            if (input.Longitude != null)
                existing.Longitude = input.Longitude;
            if (input.IsPrimary != null)
                existing.IsPrimary = input.IsPrimary;

            await context.SaveChangesAsync();

            ApiLogger.logapi("UserLocationAPI", "024", "PUTWITHSESSIONID", 1, "Test", "Test");
            return Results.Accepted($"Updated ID: {existing.Id}") as IResult;
        });

        // POST by userid
        group.MapPost("/{userid}", async (int userid, UserLocation input) =>
        {
            using var context = new EnterpriseContext();

            var userExists = context.Users.Any(u => u.Id == userid);
            if (!userExists)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "POST-USER-NOTFOUND", 1, "Test", "Test");
                return Results.NotFound("No user with that id") as IResult;
            }

            var newLocation = new UserLocation
            {
                UserId = userid,
                Label = input.Label,
                AddressLine1 = input.AddressLine1,
                AddressLine2 = input.AddressLine2,
                City = input.City,
                State = input.State,
                PostalCode = input.PostalCode,
                Country = input.Country,
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                CreatedAt = DateTime.UtcNow.ToString("o"),
                IsPrimary = input.IsPrimary
            };

            context.UserLocations.Add(newLocation);
            await context.SaveChangesAsync();

            ApiLogger.logapi("UserLocationAPI", "024", "POSTWITHUSERID", 1, "Test", "Test");
            return Results.Created($"/api/userlocation/{userid}", newLocation) as IResult;
        });

        // POST by sessionid
        group.MapPost("/session/{sessionid}", async (int sessionid, UserLocation input) =>
        {
            using var context = new EnterpriseContext();

            var session = context.Usersessions.FirstOrDefault(s => s.Id == sessionid);
            if (session == null)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "POST-SESSION-NOTFOUND", 1, "Test", "Test");
                return Results.NotFound("No session with that id") as IResult;
            }

            if (session.Userid == null)
            {
                ApiLogger.logapi("UserLocationAPI", "024", "POST-SESSION-NOUSER", 1, "Test", "Test");
                return Results.NotFound("Session has no associated user") as IResult;
            }

            var newLocation = new UserLocation
            {
                UserId = session.Userid,
                Label = input.Label,
                AddressLine1 = input.AddressLine1,
                AddressLine2 = input.AddressLine2,
                City = input.City,
                State = input.State,
                PostalCode = input.PostalCode,
                Country = input.Country,
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                CreatedAt = DateTime.UtcNow.ToString("o"),
                IsPrimary = input.IsPrimary
            };

            context.UserLocations.Add(newLocation);
            await context.SaveChangesAsync();

            ApiLogger.logapi("UserLocationAPI", "024", "POSTWITHSESSIONID", 1, "Test", "Test");
            return Results.Created($"/api/userlocation/session/{sessionid}", newLocation) as IResult;
        });
    }
}
