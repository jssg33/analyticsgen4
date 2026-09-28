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
                Challenge = session.Twofactorkey,
                Message = "FusionIdentity Root Audit Verification Required"
            });
        })
        .WithName("01GetTwoFactorChallenge")
        .WithOpenApi();


        group.MapPost("/approve/{sessionId:int}/{code}",
            async (int sessionId, string code) =>
        {
            using var context = new EnterpriseContext();

            var session = context.Usersessions
                .FirstOrDefault(x => x.Id == sessionId);

            if (session == null)
            {
                return Results.NotFound();
            }

            if (session.Twofactorkey != code)
            {
                return Results.BadRequest(
                    "FusionIdentity Error 1004: Invalid Two Factor Code");
            }

            session.Sessioncomplete = 1;
            session.Acknowledged = 1;

            await context.SaveChangesAsync();

            return Results.Ok(new
            {
                SessionId = session.Id,
                Status = "Approved"
            });
        })
        .WithName("02ApproveTwoFactor")
        .WithOpenApi();
    }
}
