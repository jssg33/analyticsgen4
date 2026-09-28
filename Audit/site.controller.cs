using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class SitesController
{
    public static void MapSitesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sites");

        group.MapGet("/", async () =>
        {
            using var db = new EnterpriseContext();

            return await db.Sites.ToListAsync();
        })
        .WithName("01GetSites")
        .WithOpenApi();

        group.MapGet("/{id}", async (int id) =>
        {
            using var db = new EnterpriseContext();

            var site = await db.Sites.FindAsync(id);

            return site is null
                ? Results.NotFound()
                : Results.Ok(site);
        })
        .WithName("02GetSite")
        .WithOpenApi();

        group.MapPost("/", async (Site site) =>
        {
            using var db = new EnterpriseContext();

            db.Sites.Add(site);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/sites/{site.Id}",
                site);
        })
        .WithName("03CreateSite")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, Site updated) =>
        {
            using var db = new EnterpriseContext();

            var existing = await db.Sites.FindAsync(id);

            if (existing is null)
            {
                return Results.NotFound();
            }

            existing.SiteName = updated.SiteName;
            existing.SiteCode = updated.SiteCode;
            existing.Address1 = updated.Address1;
            existing.Address2 = updated.Address2;
            existing.City = updated.City;
            existing.State = updated.State;
            existing.PostalCode = updated.PostalCode;
            existing.Country = updated.Country;
            existing.PhoneNumber = updated.PhoneNumber;
            existing.SiteManager = updated.SiteManager;
            existing.IsActive = updated.IsActive;
            existing.Notes = updated.Notes;

            await db.SaveChangesAsync();

            return Results.Ok(existing);
        })
        .WithName("04UpdateSite")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id) =>
        {
            using var db = new EnterpriseContext();

            var existing = await db.Sites.FindAsync(id);

            if (existing is null)
            {
                return Results.NotFound();
            }

            db.Sites.Remove(existing);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("05DeleteSite")
        .WithOpenApi();
    }
}