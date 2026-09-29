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
            return Results.Content(
$@"<!DOCTYPE html>
<html>
<head>
    <title>FusionIdentity Two Factor</title>

    <style>
        body
        {{
            font-family: Arial, Helvetica, sans-serif;
            background-color: #f5f5f5;
        }}

        .container
        {{
            width: 500px;
            margin: 100px auto;
            background: white;
            padding: 25px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0,0,0,.15);
        }}

        input
        {{
            width: 100%;
            padding: 10px;
            font-size: 18px;
            box-sizing: border-box;
        }}

        button
        {{
            padding: 10px 20px;
            font-size: 16px;
            cursor: pointer;
        }}
    </style>

    <script>
        function verifyCode()
        {{
            var code =
                document.getElementById('code').value;

            if (!code)
            {{
                alert('Please enter a verification code.');
                return;
            }}

            window.location =
                '/api/TwoFactor/approve/{sessionId}/' + code;
        }}
    </script>
</head>

<body>

    <div class='container'>

        <h2>FusionIdentity Root Audit Verification Required</h2>

        <p>
            Enter the 6 digit verification code to continue.
        </p>

        <input id='code'
               type='text'
               maxlength='6'
               autofocus />

        <br /><br />

        <button onclick='verifyCode()'>
            Verify
        </button>

    </div>

</body>
</html>",
                "text/html");
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
                    Secure = false, // Change to true once HTTPS is confirmed
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });

            return Results.Content(
$@"<!DOCTYPE html>
<html>
<head>
    <title>FusionIdentity Verification Approved</title>
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
                session.Sessionend = DateTime.UtcNow.ToString("o");

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