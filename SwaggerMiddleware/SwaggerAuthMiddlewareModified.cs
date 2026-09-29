using System.Text;
using Enterprise.Models;

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
        // Only protect Swagger
        if (!context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        // Existing approved session
        string? sessionCookie =
            context.Request.Cookies["FusionSwaggerSession"];

        if (!string.IsNullOrWhiteSpace(sessionCookie) &&
            int.TryParse(sessionCookie, out int sessionId))
        {
            using (var db = new EnterpriseContext())
            {
                var existingSession = db.Usersessions
                    .FirstOrDefault(x => x.Id == sessionId);

                if (existingSession != null &&
                    existingSession.Sessioncomplete == 1)
                {
                    await _next(context);
                    return;
                }
            }
        }

        string? authHeader =
            context.Request.Headers["Authorization"];

        if (!string.IsNullOrWhiteSpace(authHeader) &&
            authHeader.StartsWith("Basic "))
        {
            try
            {
                var encodedCredentials =
                    authHeader["Basic ".Length..].Trim();

                var decodedCredentials =
                    Encoding.UTF8.GetString(
                        Convert.FromBase64String(encodedCredentials));

                var credentials =
                    decodedCredentials.Split(':', 2);

                if (credentials.Length == 2)
                {
                    string username = credentials[0];
                    string password = credentials[1];

                    using (var db = new EnterpriseContext())
                    {
                        var user = db.Users
                            .FirstOrDefault(u =>
                                u.Username!.ToLower() ==
                                username.ToLower());

                        if (user != null)
                        {
                            bool passwordMatches =
                                BCrypt.Net.BCrypt.Verify(
                                    password,
                                    user.Hashedpassword);

                            if (passwordMatches)
                            {
                                // Role3 validation
                                if (string.IsNullOrWhiteSpace(user.Role3) ||
                                    !user.Role3.Equals(
                                        "SwaggerAdmin",
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    context.Response.StatusCode = 403;

                                    await context.Response.WriteAsync(
                                        "FusionIdentity Error 1003: SwaggerAdmin permission required.");

                                    return;
                                }

                                // Create 6 digit challenge
                                string twoFactorKey =
                                    Random.Shared
                                        .Next(100000, 999999)
                                        .ToString();

                                var session = new Usersession
                                {
                                    Userid = user.Id,
                                    Sessionusername = user.Username,
                                    Sessionemail = user.Email,
                                    Sessionfirstname = user.Firstname,
                                    Sessionlastname = user.Lastname,
                                    Sessionfullname = user.Fullname,
                                    Useridasstring = user.Id.ToString(),
                                    Sessionstart = DateTime.UtcNow.ToString("o"),
                                    Sessioncomplete = 0,
                                    Acknowledged = 0,
                                    Twofactorkey = twoFactorKey,
                                    Targetcipher = "caesar,7"
                                };

                                db.Usersessions.Add(session);
                                db.SaveChanges();

                                context.Response.StatusCode = 303;

                                context.Response.Headers.Location =
                                    $"/api/TwoFactor/{session.Id}";

                                return;
                            }
                        }
                    }
                }
            }
            catch
            {
                // intentionally fail into 401
            }
        }

        context.Response.Headers["WWW-Authenticate"] =
            "Basic realm=\"FusionIdentity Swagger\"";

        context.Response.StatusCode = 401;

        await context.Response.WriteAsync(
            "FusionIdentity Error 1001: Invalid username or password.");
    }
}