using System;
using Microsoft.EntityFrameworkCore;
using Enterprise.Models;
using EnterpriseServices;

namespace somecontrollers.Controllers;

public static class UserLocationEndpoints
{
    public static void MapUserLocationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/userlocation")
            .WithTags("UserLocation");

        // GET by userid
        group.MapGet("/{userid:int}", async (int userid) =>
        {
            using var context = new EnterpriseContext();

            var userExists = await context.Users
                .AnyAsync(u => u.Id == userid);

            if (!userExists)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "GET-USER-NOTFOUND",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound("No user with that ID.");
            }

            var locations = await context.UserLocations
                .Where(l => l.UserId == userid)
                .ToListAsync();

            ApiLogger.logapi(
                "UserLocationAPI",
                "024",
                "GETWITHUSERID",
                1,
                "Test",
                "Test");

            return Results.Ok(locations);
        });

        // GET by sessionid
        group.MapGet("/session/{sessionid:int}", async (int sessionid) =>
        {
            using var context = new EnterpriseContext();

            var session = await context.Usersessions
                .FirstOrDefaultAsync(s => s.Id == sessionid);

            if (session == null)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "GET-SESSION-NOTFOUND",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound("No session with that ID.");
            }

            if (session.Userid == null)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "GET-SESSION-NOUSER",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound("Session has no associated user.");
            }

            var locations = await context.UserLocations
                .Where(l => l.UserId == session.Userid)
                .ToListAsync();

            ApiLogger.logapi(
                "UserLocationAPI",
                "024",
                "GETWITHSESSIONID",
                1,
                "Test",
                "Test");

            return Results.Ok(locations);
        });

        // PUT by userid
        group.MapPut("/{userid:int}", async (int userid, UserLocation input) =>
        {
            using var context = new EnterpriseContext();

            var userExists = await context.Users
                .AnyAsync(u => u.Id == userid);

            if (!userExists)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "PUT-USER-NOTFOUND",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound("No user with that ID.");
            }

            var existing = await context.UserLocations
                .FirstOrDefaultAsync(l =>
                    l.UserId == userid &&
                    l.Id == input.Id);

            if (existing == null)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "PUT-LOCATION-NOTFOUND",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound(
                    $"No location {input.Id} found for user {userid}.");
            }

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

            ApiLogger.logapi(
                "UserLocationAPI",
                "024",
                "PUTWITHUSERID",
                1,
                "Test",
                "Test");

            return Results.Accepted(
                $"/api/userlocation/{userid}",
                existing);
        });

        // PUT by sessionid
        group.MapPut("/session/{sessionid:int}", async (
            int sessionid,
            UserLocation input) =>
        {
            using var context = new EnterpriseContext();

            var session = await context.Usersessions
                .FirstOrDefaultAsync(s => s.Id == sessionid);

            if (session == null)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "PUT-SESSION-NOTFOUND",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound("No session with that ID.");
            }

            if (session.Userid == null)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "PUT-SESSION-NOUSER",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound("Session has no associated user.");
            }

            var existing = await context.UserLocations
                .FirstOrDefaultAsync(l =>
                    l.UserId == session.Userid &&
                    l.Id == input.Id);

            if (existing == null)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "PUT-LOCATION-NOTFOUND",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound(
                    $"No location {input.Id} found for session {sessionid}.");
            }

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

            ApiLogger.logapi(
                "UserLocationAPI",
                "024",
                "PUTWITHSESSIONID",
                1,
                "Test",
                "Test");

            return Results.Accepted(
                $"/api/userlocation/session/{sessionid}",
                existing);
        });

        // POST by userid
        group.MapPost("/{userid:int}", async (
            int userid,
            UserLocation input) =>
        {
            using var context = new EnterpriseContext();

            var userExists = await context.Users
                .AnyAsync(u => u.Id == userid);

            if (!userExists)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "POST-USER-NOTFOUND",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound("No user with that ID.");
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

            ApiLogger.logapi(
                "UserLocationAPI",
                "024",
                "POSTWITHUSERID",
                1,
                "Test",
                "Test");

            return Results.Created(
                $"/api/userlocation/{userid}",
                newLocation);
        });

        // POST by sessionid
        group.MapPost("/session/{sessionid:int}", async (
            int sessionid,
            UserLocation input) =>
        {
            using var context = new EnterpriseContext();

            var session = await context.Usersessions
                .FirstOrDefaultAsync(s => s.Id == sessionid);

            if (session == null)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "POST-SESSION-NOTFOUND",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound("No session with that ID.");
            }

            if (session.Userid == null)
            {
                ApiLogger.logapi(
                    "UserLocationAPI",
                    "024",
                    "POST-SESSION-NOUSER",
                    1,
                    "Test",
                    "Test");

                return Results.NotFound("Session has no associated user.");
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

            ApiLogger.logapi(
                "UserLocationAPI",
                "024",
                "POSTWITHSESSIONID",
                1,
                "Test",
                "Test");

            return Results.Created(
                $"/api/userlocation/session/{sessionid}",
                newLocation);
        });
    }
}
