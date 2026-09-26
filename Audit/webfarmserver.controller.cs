using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

    public static class WebFarmServerController
    {
        public static void MapWebFarmServerEndpoints(this WebApplication app)
        {
            app.MapGet("/api/webfarmservers", () =>
            {
                using var db = new EnterpriseContext();

                return Results.Ok(db.WebFarmServers.ToList());
            })
            .WithGroupName("Web Farm Servers")
            .WithOpenApi();

            app.MapGet("/api/webfarmservers/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.WebFarmServers.FirstOrDefault(x => x.Id == id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            })
            .WithGroupName("Web Farm Servers")
            .WithOpenApi();

            app.MapPost("/api/webfarmservers", (WebFarmServer item) =>
            {
                using var db = new EnterpriseContext();

                db.WebFarmServers.Add(item);
                db.SaveChanges();

                return Results.Ok(item);
            })
            .WithGroupName("Web Farm Servers")
            .WithOpenApi();

            app.MapDelete("/api/webfarmservers/{id}", (int id) =>
            {
                using var db = new EnterpriseContext();

                var item = db.WebFarmServers.FirstOrDefault(x => x.Id == id);

                if (item == null)
                    return Results.NotFound();

                db.WebFarmServers.Remove(item);
                db.SaveChanges();

                return Results.Ok();
            })
            .WithGroupName("Web Farm Servers")
            .WithOpenApi();
        }
    }
