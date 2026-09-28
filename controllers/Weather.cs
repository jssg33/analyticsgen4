using System;
using System.Linq;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class WeatherForecastController
{
    private static readonly string[] Summaries =
    {
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    };

    public static void MapWeatherForecastEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/weather");

        // GET NO TOKEN
        group.MapGet("/", () =>
        {
            return Results.BadRequest(new
            {
                ErrorCode = 1002,
                Error = "FusionIdentity Error 1002: token required"
            });
        })
        .WithName("01GetWeatherForecastNoToken")
        .WithOpenApi();

        // GET TOKEN
        group.MapGet("/{token}", (string token) =>
        {
            using (var context = new EnterpriseContext())
            {
                var session = context.Usersessions
                    .FirstOrDefault(x => x.Token == token);

                if (session == null)
                {
                    return Results.BadRequest(new
                    {
                        ErrorCode = 1001,
                        Error = "FusionIdentity Error 1001: invalid token"
                    });
                }

                var forecast = Enumerable.Range(1, 5)
                    .Select(index => new
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        TemperatureF = 32 + (int)(Random.Shared.Next(-20, 55) / 0.5556),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    })
                    .ToArray();

                return Results.Ok(forecast);
            }
        })
        .WithName("02GetWeatherForecast")
        .WithOpenApi();

        // POST NO TOKEN
        group.MapPost("/", () =>
        {
            return Results.BadRequest(new
            {
                ErrorCode = 1002,
                Error = "FusionIdentity Error 1002: token required"
            });
        })
        .WithName("03CreateWeatherForecastNoToken")
        .WithOpenApi();

        // POST TOKEN
        group.MapPost("/{token}", (string token) =>
        {
            using (var context = new EnterpriseContext())
            {
                var session = context.Usersessions
                    .FirstOrDefault(x => x.Token == token);

                if (session == null)
                {
                    return Results.BadRequest(new
                    {
                        ErrorCode = 1001,
                        Error = "FusionIdentity Error 1001: invalid token"
                    });
                }

                return Results.Ok("POST Success");
            }
        })
        .WithName("04CreateWeatherForecast")
        .WithOpenApi();

        // PUT NO TOKEN
        group.MapPut("/{id}", (int id) =>
        {
            return Results.BadRequest(new
            {
                ErrorCode = 1002,
                Error = "FusionIdentity Error 1002: token required"
            });
        })
        .WithName("05UpdateWeatherForecastNoToken")
        .WithOpenApi();

        // PUT TOKEN
        group.MapPut("/{id}/{token}", (int id, string token) =>
        {
            using (var context = new EnterpriseContext())
            {
                var session = context.Usersessions
                    .FirstOrDefault(x => x.Token == token);

                if (session == null)
                {
                    return Results.BadRequest(new
                    {
                        ErrorCode = 1001,
                        Error = "FusionIdentity Error 1001: invalid token"
                    });
                }

                return Results.Ok("PUT Success");
            }
        })
        .WithName("06UpdateWeatherForecast")
        .WithOpenApi();

        // DELETE NO TOKEN
        group.MapDelete("/{id}", (int id) =>
        {
            return Results.BadRequest(new
            {
                ErrorCode = 1002,
                Error = "FusionIdentity Error 1002: token required"
            });
        })
        .WithName("07DeleteWeatherForecastNoToken")
        .WithOpenApi();

        // DELETE TOKEN
        group.MapDelete("/{id}/{token}", (int id, string token) =>
        {
            using (var context = new EnterpriseContext())
            {
                var session = context.Usersessions
                    .FirstOrDefault(x => x.Token == token);

                if (session == null)
                {
                    return Results.BadRequest(new
                    {
                        ErrorCode = 1001,
                        Error = "FusionIdentity Error 1001: invalid token"
                    });
                }

                return Results.Ok("DELETE Success");
            }
        })
        .WithName("08DeleteWeatherForecast")
        .WithOpenApi();
    }
}
