using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

    public static class WebFarmController
    {
        public static void MapWebFarmEndpoints(this WebApplication app)
        {
            app.MapGet("/api/webfarms", () =>
            {
                using var db = new EnterpriseContext();
                return Results.Ok(db.WebFarms.ToList());
            })
            .WithGroupName("Web Farms")
            .WithOpenApi();

            app.MapGet("/api/webfarms/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.WebFarms.FirstOrDefault(x => x.Id == id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            })
            .WithGroupName("Web Farms")
            .WithOpenApi();

            app.MapPost("/api/webfarms", (WebFarm item) =>
            {
                using var db = new EnterpriseContext();

                db.WebFarms.Add(item);
                db.SaveChanges();

                return Results.Ok(item);
            })
            .WithGroupName("Web Farms")
            .WithOpenApi();

            app.MapPut("/api/webfarms/{id}", (int id, WebFarm updated) =>
            {
                using var db = new EnterpriseContext();

                var item = db.WebFarms.FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                item.InventoryId = updated.InventoryId;
                item.FarmName = updated.FarmName;
                item.Description = updated.Description;

                item.LoadBalancerName = updated.LoadBalancerName;
                item.LoadBalancerType = updated.LoadBalancerType;
                item.VirtualIpAddress = updated.VirtualIpAddress;

                item.Url = updated.Url;

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

                item.BackendPoolCount = updated.BackendPoolCount;
                item.HealthProbeUrl = updated.HealthProbeUrl;

                item.PublicFacing = updated.PublicFacing;
                item.Enabled = updated.Enabled;

                item.Notes = updated.Notes;

                db.SaveChanges();

                return Results.Ok(item);
            })
            .WithGroupName("Web Farms")
            .WithOpenApi();

            app.MapDelete("/api/webfarms/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.WebFarms.FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                db.WebFarms.Remove(item);
                db.SaveChanges();

                return Results.Ok();
            })
            .WithGroupName("Web Farms")
            .WithOpenApi();
        }
    }
