using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApiApplicationsAccessController
{
    public static void MapApplicationAccessEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/applicationaccess");

        group.MapGet("/", async (EnterpriseContext db) =>
        {
            return await db.ApplicationUsers.ToListAsync();
        })
        .WithName("01GetAllApplicationAccess")
        .WithOpenApi();

        group.MapGet("/{id:int}", async (int id, EnterpriseContext db) =>
        {
            var item = await db.ApplicationUsers.FindAsync(id);

            if (item == null)
                return Results.NotFound();

            return Results.Ok(item);
        })
        .WithName("01GetApplicationAccessById")
        .WithOpenApi();

        group.MapGet("/application/{applicationId:int}", async (int applicationId, EnterpriseContext db) =>
        {
            var items = await db.ApplicationUsers
                .Where(x => x.ApplicationId == applicationId)
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("01GetApplicationAccessByApplication")
        .WithOpenApi();

        group.MapGet("/user/{userId:int}", async (int userId, EnterpriseContext db) =>
        {
            var items = await db.ApplicationUsers
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return Results.Ok(items);
        })
        .WithName("01GetApplicationAccessByUser")
        .WithOpenApi();

        group.MapPost("/", async (ApplicationUser input, EnterpriseContext db) =>
        {
            input.CreatedDate = DateTime.UtcNow;

            db.ApplicationUsers.Add(input);

            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/applicationaccess/{input.Id}",
                input);
        })
        .WithName("01CreateApplicationAccess")
        .WithOpenApi();

        group.MapPut("/{id:int}", async (int id, ApplicationUser input, EnterpriseContext db) =>
        {
            var existing = await db.ApplicationUsers.FindAsync(id);

            if (existing == null)
                return Results.NotFound();

            existing.ApplicationId = input.ApplicationId;
            existing.UserId = input.UserId;

            if (input.Permission != null)
                existing.Permission = input.Permission;

            if (input.RoleName != null)
                existing.RoleName = input.RoleName;

            if (input.AccessType != null)
                existing.AccessType = input.AccessType;

            if (input.ApprovalGroup != null)
                existing.ApprovalGroup = input.ApprovalGroup;

            if (input.ReviewedBy != null)
                existing.ReviewedBy = input.ReviewedBy;

            if (input.RequestTicket != null)
                existing.RequestTicket = input.RequestTicket;

            if (input.BusinessJustification != null)
                existing.BusinessJustification = input.BusinessJustification;

            existing.IsActive = input.IsActive;

            if (input.AccessGrantedDate.HasValue)
                existing.AccessGrantedDate = input.AccessGrantedDate;

            if (input.AccessRemovedDate.HasValue)
                existing.AccessRemovedDate = input.AccessRemovedDate;

            if (input.LastReviewedDate.HasValue)
                existing.LastReviewedDate = input.LastReviewedDate;

            existing.ModifiedDate = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(existing);
        })
        .WithName("01UpdateApplicationAccess")
        .WithOpenApi();

        group.MapDelete("/{id:int}", async (int id, EnterpriseContext db) =>
        {
            var existing = await db.ApplicationUsers.FindAsync(id);

            if (existing == null)
                return Results.NotFound();

            db.ApplicationUsers.Remove(existing);

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("01DeleteApplicationAccess")
        .WithOpenApi();
    }
}