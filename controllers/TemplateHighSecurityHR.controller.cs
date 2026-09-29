using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FusionIdentity.Data;
using FusionIdentity.Models;

namespace FusionIdentity.Controllers
{
    public static class HRController
    {
        public static void MapHREndpoints(this WebApplication app)
        {
            app.MapGet("/api/hr", async () =>
            {
                using var db = new EnterpriseContext();

                var employees = await db.EmployeeHR
                    .AsNoTracking()
                    .ToListAsync();

                return Results.Ok(employees);

            })
            .WithName("HR")
            .WithOpenApi();

            app.MapGet("/api/hr/{username}", async (string username) =>
            {
                using var db = new EnterpriseContext();

                var employee = await db.EmployeeHR
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Username == username);

                if (employee == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(employee);

            })
            .WithName("HRByUser")
            .WithOpenApi();

            app.MapGet("/api/hr/whoami", async () =>
            {
                using var db = new EnterpriseContext();

                var connection = db.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using var command = connection.CreateCommand();

                command.CommandText = @"
                    SELECT
                        CURRENT_USER,
                        USER_NAME(),
                        ORIGINAL_LOGIN()
                ";

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return Results.BadRequest();
                }

                return Results.Ok(new
                {
                    CurrentUser = reader[0]?.ToString(),
                    UserName = reader[1]?.ToString(),
                    OriginalLogin = reader[2]?.ToString()
                });

            })
            .WithName("HRWhoAmI")
            .WithOpenApi();
        }
    }
}
