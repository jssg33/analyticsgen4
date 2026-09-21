using Enterprise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class KeyServerEndpoints
{
    public static void MapKeyServerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/pki").WithTags("PKI Keys");

        // Issue a key for a user session
        group.MapPost("/session-keys", async ([FromBody] KeyIssueRequest req) =>
        {
            using var context = new EnterpriseContext();

            var session = await context.Usersessions.FindAsync(req.SessionId);
            if (session == null)
                return Results.NotFound(new { message = "Session not found" });

            if (session.Sessioncomplete == 1)
                return Results.BadRequest(new { message = "Session is closed" });

            string publicKey = "";
            string? privateKey = null;

            switch (req.KeyType.ToLower())
            {
                case "rsa":
                    var rsa = BouncyKeyGenerator.GenerateRsa(req.KeySize ?? 2048);
                    publicKey = rsa.publicPem;
                    privateKey = rsa.privatePem;
                    break;

                case "ec":
                    var ec = BouncyKeyGenerator.GenerateEc();
                    publicKey = ec.publicPem;
                    privateKey = ec.privatePem;
                    break;

                case "ed25519":
                    var ed = BouncyKeyGenerator.GenerateEd25519();
                    publicKey = ed.publicPem;
                    privateKey = ed.privatePem;
                    break;

                case "aes":
                    publicKey = BouncyKeyGenerator.GenerateAes(req.KeySize ?? 256);
                    break;

                case "des":
                    publicKey = BouncyKeyGenerator.GenerateDes();
                    break;

                case "3des":
                    publicKey = BouncyKeyGenerator.GenerateTripleDes();
                    break;

                default:
                    return Results.BadRequest(new { message = "Unsupported key type" });
            }

            var assignment = new Keyassignment
            {
                SessionId = req.SessionId,
                KeyId = Guid.NewGuid().ToString(),
                Alias = req.Alias,
                PublicKey = publicKey,
                PrivateKey = privateKey,
                DeliveredToUser = 0,
                CreatedAt = DateTime.UtcNow.ToString("o"),
                UpdatedAt = DateTime.UtcNow.ToString("o")
            };

            context.Keyassignments.Add(assignment);
            await context.SaveChangesAsync();

            return Results.Created($"/api/pki/session-keys/{assignment.Id}", new
            {
                assignment.Id,
                assignment.SessionId,
                assignment.KeyId,
                assignment.Alias,
                assignment.PublicKey,
                assignment.CreatedAt
            });
        });

        // Get all keys for a session
        group.MapGet("/session-keys/{sessionId}", async (int sessionId) =>
        {
            using var context = new EnterpriseContext();

            var session = await context.Usersessions.FindAsync(sessionId);
            if (session == null)
                return Results.NotFound(new { message = "Session not found" });

            var keys = await context.Keyassignments
                .Where(k => k.SessionId == sessionId)
                .Select(k => new
                {
                    k.KeyId,
                    k.Alias,
                    k.PublicKey,
                    k.CreatedAt
                })
                .ToListAsync();

            return Results.Ok(keys);
        });

        // Delete a key
        group.MapDelete("/keys/{id}", async (int id) =>
        {
            using var context = new EnterpriseContext();

            var key = await context.Keyassignments.FindAsync(id);
            if (key == null)
                return Results.NotFound(new { message = "Key not found" });

            context.Keyassignments.Remove(key);
            await context.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}

public record KeyIssueRequest(
    int SessionId,
    string KeyType,
    string? Alias,
    int? KeySize
);
