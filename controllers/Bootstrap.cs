using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

    public static class BootstrapController
    {
        public static void MapBootstrapEndpoints(this WebApplication app)
        {
            app.MapGet("/api/bootstrap/time", () =>
            {
                return Results.Ok(new
                {
                    ServerTime = DateTime.UtcNow,
                    ServerTimeLocal = DateTime.Now,
                    TimeZone = TimeZoneInfo.Local.DisplayName
                });
            })
            .WithName("GetBootstrap")
            .WithSummary("Returns server time for bootstrap validation")
            .WithDescription("Simple bootstrap endpoint used to verify API availability and obtain server time.")
            .WithOpenApi();

            // OpenAPI Group
            // Bootstrap
        }
    }
