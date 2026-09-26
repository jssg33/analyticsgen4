using Enterprise.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Controllers;

public static class UserDownloadLogEndpoints
{
    public static void MapUserDownloadLogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/UserDownloadLogs")
            .WithTags(nameof(UserDownloadLog));

        group.MapGet("/", async () =>
        {
            using var context = new EnterpriseContext();

            return await context.UserDownloadLogs
                .AsNoTracking()
                .OrderByDescending(x => x.DownloadDateTime)
                .ToListAsync();
        })
        .WithName("GetAllUserDownloadLogs")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<UserDownloadLog>, NotFound>> (int id) =>
        {
            using var context = new EnterpriseContext();

            var item = await context.UserDownloadLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(item);
        })
        .WithName("GetUserDownloadLogById")
        .WithOpenApi();

        group.MapGet("/username/{username}", async (string username) =>
        {
            using var context = new EnterpriseContext();

            return await context.UserDownloadLogs
                .AsNoTracking()
                .Where(x => x.Username == username)
                .OrderByDescending(x => x.DownloadDateTime)
                .ToListAsync();
        })
        .WithName("GetUserDownloadLogsByUsername")
        .WithOpenApi();

        group.MapGet("/application/{application}", async (string application) =>
        {
            using var context = new EnterpriseContext();

            return await context.UserDownloadLogs
                .AsNoTracking()
                .Where(x => x.SourceApplication == application)
                .OrderByDescending(x => x.DownloadDateTime)
                .ToListAsync();
        })
        .WithName("GetUserDownloadLogsByApplication")
        .WithOpenApi();

        group.MapGet("/file/{fileName}", async (string fileName) =>
        {
            using var context = new EnterpriseContext();

            return await context.UserDownloadLogs
                .AsNoTracking()
                .Where(x => x.FileName == fileName)
                .OrderByDescending(x => x.DownloadDateTime)
                .ToListAsync();
        })
        .WithName("GetUserDownloadLogsByFile")
        .WithOpenApi();

        group.MapGet("/failed", async () =>
        {
            using var context = new EnterpriseContext();

            return await context.UserDownloadLogs
                .AsNoTracking()
                .Where(x => !x.Successful)
                .OrderByDescending(x => x.DownloadDateTime)
                .ToListAsync();
        })
        .WithName("GetFailedUserDownloadLogs")
        .WithOpenApi();

        group.MapPost("/", async Task<Created<UserDownloadLog>> (UserDownloadLog input) =>
        {
            using var context = new EnterpriseContext();

            input.DownloadDateTime = DateTime.UtcNow;

            context.UserDownloadLogs.Add(input);
            await context.SaveChangesAsync();

            return TypedResults.Created(
                $"/api/UserDownloadLogs/{input.Id}",
                input);
        })
        .WithName("CreateUserDownloadLog")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<NoContent, NotFound>> (int id, UserDownloadLog input) =>
        {
            using var context = new EnterpriseContext();

            var item = await context.UserDownloadLogs.FindAsync(id);

            if (item is null)
            {
                return TypedResults.NotFound();
            }

            item.Username = input.Username;
            item.Uid = input.Uid;
            item.UserEmail = input.UserEmail;
            item.FileName = input.FileName;
            item.FilePath = input.FilePath;
            item.FileSizeBytes = input.FileSizeBytes;
            item.ContentType = input.ContentType;
            item.SourceApplication = input.SourceApplication;
            item.DownloadMethod = input.DownloadMethod;
            item.UserIPAddress = input.UserIPAddress;
            item.UserLocation = input.UserLocation;
            item.UserAgent = input.UserAgent;
            item.Successful = input.Successful;
            item.FailureReason = input.FailureReason;

            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .WithName("UpdateUserDownloadLog")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<NoContent, NotFound>> (int id) =>
        {
            using var context = new EnterpriseContext();

            var item = await context.UserDownloadLogs.FindAsync(id);

            if (item is null)
            {
                return TypedResults.NotFound();
            }

            context.UserDownloadLogs.Remove(item);
            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .WithName("DeleteUserDownloadLog")
        .WithOpenApi();

        group.MapGet("/statistics", async () =>
        {
            using var context = new EnterpriseContext();

            var totalDownloads = await context.UserDownloadLogs.CountAsync();

            var successfulDownloads =
                await context.UserDownloadLogs.CountAsync(x => x.Successful);

            var failedDownloads =
                await context.UserDownloadLogs.CountAsync(x => !x.Successful);

            var totalBytesDownloaded =
                await context.UserDownloadLogs
                    .Where(x => x.Successful)
                    .SumAsync(x => (long?)x.FileSizeBytes) ?? 0;

            return TypedResults.Ok(new
            {
                TotalDownloads = totalDownloads,
                SuccessfulDownloads = successfulDownloads,
                FailedDownloads = failedDownloads,
                TotalBytesDownloaded = totalBytesDownloaded
            });
        })
        .WithName("GetUserDownloadLogStatistics")
        .WithOpenApi();
    }
}