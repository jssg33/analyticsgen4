using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

    public static class WorkerTroubleTicketController
    {
        public static void MapWorkerTroubleTicketEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/WorkerTroubleTickets")
                .WithTags("Audit")
                .WithOpenApi();

            // GET ALL
            group.MapGet("/", async () =>
            {
                using var context = new EnterpriseContext();

                var records = await context.WorkerTroubleTickets
                    .OrderByDescending(x => x.ReportedOn)
                    .ToListAsync();

                return Results.Ok(records);
            })
            .WithName("GetWorkerTroubleTickets");

            // GET BY ID
            group.MapGet("/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var record = await context.WorkerTroubleTickets
                    .FirstOrDefaultAsync(x => x.Id == id);

                return record == null
                    ? Results.NotFound()
                    : Results.Ok(record);
            })
            .WithName("GetWorkerTroubleTicket");

            // GET OPEN TICKETS
            group.MapGet("/open", async () =>
            {
                using var context = new EnterpriseContext();

                var records = await context.WorkerTroubleTickets
                    .Where(x => x.Status == "Open")
                    .OrderByDescending(x => x.ReportedOn)
                    .ToListAsync();

                return Results.Ok(records);
            })
            .WithName("GetOpenWorkerTroubleTickets");

            // GET HARD DOWN
            group.MapGet("/harddown", async () =>
            {
                using var context = new EnterpriseContext();

                var records = await context.WorkerTroubleTickets
                    .Where(x => x.IsHardDown)
                    .OrderByDescending(x => x.ReportedOn)
                    .ToListAsync();

                return Results.Ok(records);
            })
            .WithName("GetHardDownWorkerTroubleTickets");

            // POST CREATE
            group.MapPost("/", async (WorkerTroubleTicket input) =>
            {
                using var context = new EnterpriseContext();

                input.ReportedOn = DateTime.UtcNow;

                if (string.IsNullOrWhiteSpace(input.Status))
                    input.Status = "Open";

                if (string.IsNullOrWhiteSpace(input.Severity))
                    input.Severity = "Critical";

                context.WorkerTroubleTickets.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/WorkerTroubleTickets/{input.Id}",
                    input);
            })
            .WithName("CreateWorkerTroubleTicket");

            // PUT UPDATE
            group.MapPut("/{id:int}", async (int id, WorkerTroubleTicket input) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.WorkerTroubleTickets
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (existing == null)
                    return Results.NotFound();

                existing.ApiName = input.ApiName;
                existing.Endpoint = input.Endpoint;
                existing.Environment = input.Environment;
                existing.Severity = input.Severity;
                existing.Status = input.Status;
                existing.ReportedBy = input.ReportedBy;
                existing.Description = input.Description;
                existing.AssignedTo = input.AssignedTo;
                existing.ResolutionNotes = input.ResolutionNotes;
                existing.ResolvedOn = input.ResolvedOn;
                existing.IsHardDown = input.IsHardDown;
                existing.IncidentCount = input.IncidentCount;
                existing.ImpactedUsers = input.ImpactedUsers;
                existing.BusinessUnit = input.BusinessUnit;
                existing.RootCause = input.RootCause;
                existing.EvidenceUrl = input.EvidenceUrl;
                existing.AuditorNotes = input.AuditorNotes;
                existing.ApplicationOwner = input.ApplicationOwner;

                await context.SaveChangesAsync();

                return Results.Ok(existing);
            })
            .WithName("UpdateWorkerTroubleTicket");

            // RESOLVE TICKET
            group.MapPut("/{id:int}/resolve", async (int id, string resolutionNotes) =>
            {
                using var context = new EnterpriseContext();

                var ticket = await context.WorkerTroubleTickets
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (ticket == null)
                    return Results.NotFound();

                ticket.Status = "Resolved";
                ticket.ResolvedOn = DateTime.UtcNow;
                ticket.ResolutionNotes = resolutionNotes;

                await context.SaveChangesAsync();

                return Results.Ok(ticket);
            })
            .WithName("ResolveWorkerTroubleTicket");

            // DELETE
            group.MapDelete("/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var record = await context.WorkerTroubleTickets
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (record == null)
                    return Results.NotFound();

                context.WorkerTroubleTickets.Remove(record);

                await context.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = "Ticket deleted successfully.",
                    Id = id
                });
            })
            .WithName("DeleteWorkerTroubleTicket");
        }
    }
