using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

    public static class WebServerController
    {
        public static void MapWebServerEndpoints(this WebApplication app)
        {
            app.MapGet("/api/webservers", () =>
            {
                using var db = new EnterpriseContext();
                return Results.Ok(db.WebServers.ToList());
            })
            .WithGroupName("Web Servers")
            .WithOpenApi();

            app.MapGet("/api/webservers/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.WebServers.FirstOrDefault(x => x.Id == id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            })
            .WithGroupName("Web Servers")
            .WithOpenApi();

            app.MapPost("/api/webservers", (WebServer item) =>
            {
                using var db = new EnterpriseContext();

                db.WebServers.Add(item);
                db.SaveChanges();

                return Results.Ok(item);
            })
            .WithGroupName("Web Servers")
            .WithOpenApi();

            app.MapPut("/api/webservers/{id}", (int id, WebServer updated) =>
            {
                using var db = new EnterpriseContext();

                var item = db.WebServers.FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                item.InventoryId = updated.InventoryId;
                item.ServerName = updated.ServerName;
                item.Hostname = updated.Hostname;
                item.IpAddress = updated.IpAddress;
                item.OperatingSystem = updated.OperatingSystem;
                item.WebServerType = updated.WebServerType;
                item.Version = updated.Version;

                item.AzureURL = updated.AzureURL;
                item.AzureIP = updated.AzureIP;

                item.CName1 = updated.CName1;
                item.CName2 = updated.CName2;
                item.CName3 = updated.CName3;

                item.ApplicationURL = updated.ApplicationURL;

                item.Environment = updated.Environment;
                item.Location = updated.Location;

                item.AzureSubscription = updated.AzureSubscription;
                item.ResourceGroup = updated.ResourceGroup;
                item.Region = updated.Region;

                item.Owner = updated.Owner;
                item.SupportTeam = updated.SupportTeam;

                item.TLSVersion = updated.TLSVersion;

                item.PublicFacing = updated.PublicFacing;
                item.Active = updated.Active;

                item.Notes = updated.Notes;

                db.SaveChanges();

                return Results.Ok(item);
            })
            .WithGroupName("Web Servers")
            .WithOpenApi();

            app.MapDelete("/api/webservers/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.WebServers.FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                db.WebServers.Remove(item);
                db.SaveChanges();

                return Results.Ok();
            })
            .WithGroupName("Web Servers")
            .WithOpenApi();
        }
    }
