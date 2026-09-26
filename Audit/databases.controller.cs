using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

    public static class DatabaseController
    {
        public static void MapDatabaseEndpoints(this WebApplication app)
        {
            app.MapGet("/api/databases", () =>
            {
                using var db = new EnterpriseContext();
                return Results.Ok(db.Databases.ToList());
            })
            .WithGroupName("Databases")
            .WithOpenApi();

            app.MapGet("/api/databases/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.Databases.FirstOrDefault(x => x.Id == id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            })
            .WithGroupName("Databases")
            .WithOpenApi();

            app.MapPost("/api/databases", (Database item) =>
            {
                using var db = new EnterpriseContext();

                db.Databases.Add(item);
                db.SaveChanges();

                return Results.Ok(item);
            })
            .WithGroupName("Databases")
            .WithOpenApi();

            app.MapPut("/api/databases/{id}", (int id, Database updated) =>
            {
                using var db = new EnterpriseContext();

                var item = db.Databases.FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                item.InventoryId = updated.InventoryId;
                item.DatabaseServerId = updated.DatabaseServerId;
                item.DatabaseName = updated.DatabaseName;
                item.Description = updated.Description;
                item.DatabaseType = updated.DatabaseType;
                item.Version = updated.Version;
                item.Classification = updated.Classification;
                item.Owner = updated.Owner;
                item.SupportTeam = updated.SupportTeam;
                item.BackupRequired = updated.BackupRequired;
                item.EncryptionEnabled = updated.EncryptionEnabled;
                item.ContainsPII = updated.ContainsPII;
                item.ContainsPHI = updated.ContainsPHI;
                item.ContainsPCI = updated.ContainsPCI;
                item.Active = updated.Active;
                item.Notes = updated.Notes;

                db.SaveChanges();

                return Results.Ok(item);
            })
            .WithGroupName("Databases")
            .WithOpenApi();

            app.MapDelete("/api/databases/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.Databases.FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                db.Databases.Remove(item);
                db.SaveChanges();

                return Results.Ok();
            })
            .WithGroupName("Databases")
            .WithOpenApi();
        }
    }
