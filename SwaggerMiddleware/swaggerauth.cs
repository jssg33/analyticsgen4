using System.Text;
using Enterprise.Models;
using EnterpriseServices;

namespace SwaggerTools;

public class SwaggerAuthMiddleware
{
    private readonly RequestDelegate _next;

    public SwaggerAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            string? authHeader =
                context.Request.Headers["Authorization"]
                .ToString();

            if (!string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith("Basic "))
            {
                try
                {
                    string encoded =
                        authHeader.Substring("Basic ".Length)
                        .Trim();

                    string decoded =
                        Encoding.UTF8.GetString(
                            Convert.FromBase64String(encoded));

                    string[] parts =
                        decoded.Split(':', 2);

                    if (parts.Length == 2)
                    {
                        string username = parts[0];
                        string password = parts[1];

                        using var db = new EnterpriseContext();

                        var user = db.Users
                            .FirstOrDefault(u =>
                                u.Username == username);

                        if (user != null)
                        {
                            bool validPassword =
                                BCrypt.Net.BCrypt.Verify(
                                    password,
                                    user.Hashedpassword);

                            bool isSwaggerAdmin =
                                user.Role3 == "SwaggerAdmin";

                            if (validPassword && isSwaggerAdmin)
                            {
                                var twoFactor =
                                    new Twofactor();

                                string challenge =
                                    twoFactor.GenerateKey();

                                var session =
                                    new Usersession
                                    {
                                        Userid = user.Id,
                                        Useridasstring =
                                            user.Id.ToString(),

                                        Token =
                                            Guid.NewGuid()
                                            .ToString("N"),

                                        Sessionstart =
                                            DateTime.UtcNow
                                            .ToString("o"),

                                        Sessionend = null,

                                        Sessiondescription =
                                            "Swagger Login Pending 2FA",

                                        Sessionusername =
                                            user.Username,

                                        Sessionemail =
                                            user.Email,

                                        Sessionfirstname =
                                            user.Firstname,

                                        Sessionlastname =
                                            user.Lastname,

                                        Sessionfullname =
                                            user.Fullname,

                                        Sessioncomplete = 0,

                                        Acknowledged = 0,

                                        Twofactorkey =
                                            challenge
                                    };

                                db.Usersessions.Add(session);
                                db.SaveChanges();

                                context.Response.StatusCode =
                                    StatusCodes.Status303SeeOther;

                                context.Response.Headers.Location =
                                    $"/api/twofactor/{session.Id}";

                                return;
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore malformed auth header
                }
            }

            context.Response.Headers["WWW-Authenticate"] =
                "Basic";

            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsync(
                "FusionIdentity Error 1001: Invalid Credentials");

            return;
        }

        await _next(context);
    }
}
