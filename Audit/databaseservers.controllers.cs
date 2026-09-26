using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

    public static class DatabaseServerController
    {
        public static void MapDatabaseServerEndpoints(this WebApplication app)
        {
            app.MapGet("/api/databaseservers", () =>
            {
                using var db = new EnterpriseContext();
                return Results.Ok(db.DatabaseServers.ToList());
            })
            .WithGroupName("Database Servers")
            .WithOpenApi();

            app.MapGet("/api/databaseservers/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.DatabaseServers.FirstOrDefault(x => x.Id == id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            })
            .WithGroupName("Database Servers")
            .WithOpenApi();

            app.MapPost("/api/databaseservers", (DatabaseServer item) =>
            {
                using var db = new EnterpriseContext();

                db.DatabaseServers.Add(item);
                db.SaveChanges();

                return Results.Ok(item);
            })
            .WithGroupName("Database Servers")
            .WithOpenApi();

            app.MapPut("/api/databaseservers/{id}", (int id, DatabaseServer updated) =>
            {
                using var db = new EnterpriseContext();

                var item = db.DatabaseServers.FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                item.InventoryId = updated.InventoryId;
                item.ServerName = updated.ServerName;
                item.Hostname = updated.Hostname;
                item.IpAddress = updated.IpAddress;
                item.DatabasePlatform = updated.DatabasePlatform;
                item.Version = updated.Version;
                item.DatabaseInstance = updated.DatabaseInstance;

                item.AzureURL = updated.AzureURL;
                item.AzureIP = updated.AzureIP;

                item.CName1 = updated.CName1;
                item.CName2 = updated.CName2;
                item.CName3 = updated.CName3;

                item.Environment = updated.Environment;
                item.Location = updated.Location;

                item.AzureSubscription = updated.AzureSubscription;
                item.ResourceGroup = updated.ResourceGroup;
                item.Region = updated.Region;

                item.Owner = updated.Owner;
                item.SupportTeam = updated.SupportTeam;

                item.BackupSolution = updated.BackupSolution;
                item.RecoveryModel = updated.RecoveryModel;

                item.PublicFacing = updated.PublicFacing;
                item.Active = updated.Active;
                item.Notes = updated.Notes;

                db.SaveChanges();

                return Results.Ok(item);
            })
            .WithGroupName("Database Servers")
            .WithOpenApi();

            app.MapDelete("/api/databaseservers/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.DatabaseServers.FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                db.DatabaseServers.Remove(item);
                db.SaveChanges();

                return Results.Ok();
            })
            .WithGroupName("Database Servers")
            .WithOpenApi();
        }
    }
