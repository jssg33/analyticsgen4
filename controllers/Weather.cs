using Enterprise.Data;

namespace Enterprise.Controllers;

public static class WeatherForecastController
{
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

                return Results.Ok("GET Success");
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
