using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class TwofactorEndpoints
{
    public static void MapTwofactorEndpoints(
        this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TwoFactor")
            .WithTags("TwoFactor");

        group.MapGet("/{sessionId:int}", (int sessionId) =>
        {
            using var context = new EnterpriseContext();

            var session = context.Usersessions
                .FirstOrDefault(x => x.Id == sessionId);

            if (session == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new
            {
                ErrorCode = 1003,
                Status = "TwoFactorRequired",
                SessionId = session.Id,
                UserId = session.Userid,

                // REMOVE THIS LATER WHEN SMS/EMAIL IS IMPLEMENTED
                Challenge = session.Twofactorkey,

                Message = "FusionIdentity Root Audit Verification Required"
            });
        })
        .WithName("01GetTwoFactorChallenge")
        .WithOpenApi();

group.MapGet(
"/approve/{sessionId:int}/{code}",
async (
int sessionId,
string code,
HttpContext httpContext) =>
{
using var context = new EnterpriseContext();
 
var session = context.Usersessions
.FirstOrDefault(x => x.Id == sessionId);
 
if (session == null)
{
return Results.NotFound(
$"FusionIdentity Error 1005: Session {sessionId} not found.");
}
 
if (!string.Equals(
session.Twofactorkey?.Trim(),
code?.Trim(),
StringComparison.Ordinal))
{
return Results.BadRequest(
"FusionIdentity Error 1004: Invalid Two Factor Code");
}
 
session.Sessioncomplete = 1;
session.Acknowledged = 1;
session.Sessionend = DateTime.UtcNow.ToString("o");
 
context.Usersessions.Update(session);
 
var rows = await context.SaveChangesAsync();
 
if (rows == 0)
{
return Results.BadRequest(
"FusionIdentity Error 1006: Session update failed.");
}
 
httpContext.Response.Cookies.Append(
"FusionSwaggerSession",
session.Id.ToString(),
new CookieOptions
{
HttpOnly = true,
Secure = false, // set true once HTTPS is confirmed
SameSite = SameSiteMode.Lax,
Expires = DateTimeOffset.UtcNow.AddHours(8)
});
 
return Results.Content(
$@"<html>
<head>
<title>FusionIdentity</title>
<meta http-equiv='refresh' content='2;url=/swagger' />
</head>
<body>
<h2>FusionIdentity Verification Approved</h2>
<p>Session {session.Id} approved.</p>
<p>Rows Updated: {rows}</p>
<p>Redirecting to Swagger...</p>
</body>
</html>",
"text/html");
})
.WithName("02ApproveTwoFactor")
.WithOpenApi();

        group.MapGet(
            "/swaggerlogout/{sessionId:int}",
            async (
                int sessionId,
                HttpContext httpContext) =>
        {
            using var context = new EnterpriseContext();

            var session = context.Usersessions
                .FirstOrDefault(x => x.Id == sessionId);

            if (session != null)
            {
                session.Sessioncomplete = 0;
                session.Acknowledged = 0;
                session.Sessionend =
                    DateTime.UtcNow.ToString("o");

                await context.SaveChangesAsync();
            }

            httpContext.Response.Cookies.Delete(
                "FusionSwaggerSession");

            return Results.Ok(new
            {
                Status = "LoggedOut",
                SessionId = sessionId
            });
        })
        .WithName("03SwaggerLogout")
        .WithOpenApi();
    }
}