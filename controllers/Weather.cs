using Microsoft.AspNetCore.Mvc;

namespace Enterprise.Controllers;

public static class WeatherForecastController
{
    private static readonly string[] Summaries =
    [
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
    ];

    public static void MapWeatherForecastEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/weather");

        group.MapGet("/", (
            [FromQuery] string token) =>
        {
            var forecast = Enumerable.Range(1, 5)
                .Select(index => new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
                .ToArray();

            return Results.Ok(new
            {
                Token = token,
                Forecast = forecast
            });
        })
        .WithName("01GetWeatherForecast")
        .WithOpenApi();
    }

    public record WeatherForecast(
        DateOnly Date,
        int TemperatureC,
        string? Summary)
    {
        public int TemperatureF =>
            32 + (int)(TemperatureC / 0.5556);
    }
}
