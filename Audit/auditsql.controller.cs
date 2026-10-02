using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

    public static class AuditSqlController
    {
        public static void MapAuditSqlEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/AuditSql")
                .WithTags("Audit SQL")
                .WithOpenApi();

            #region AuditSqlServers

            group.MapGet("/Servers", async () =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditSqlServers
                        .OrderBy(x => x.ServerName)
                        .ToListAsync());
            });

            group.MapGet("/Servers/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var item = await context.AuditSqlServers.FindAsync(id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            });

            group.MapPost("/Servers", async (AuditSqlServer input) =>
            {
                using var context = new EnterpriseContext();

                context.AuditSqlServers.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/AuditSql/Servers/{input.Id}",
                    input);
            });

            group.MapPut("/Servers/{id:int}", async (int id, AuditSqlServer input) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditSqlServers.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.Entry(existing).CurrentValues.SetValues(input);

                await context.SaveChangesAsync();

                return Results.Ok(existing);
            });

            group.MapDelete("/Servers/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditSqlServers.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.AuditSqlServers.Remove(existing);

                await context.SaveChangesAsync();

                return Results.Ok();
            });

            #endregion

            #region AuditSqlTables

            group.MapGet("/Tables", async () =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditSqlTables
                        .OrderBy(x => x.TableName)
                        .ToListAsync());
            });

            group.MapGet("/Tables/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var item = await context.AuditSqlTables.FindAsync(id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            });

            group.MapGet("/Servers/{serverId:int}/Tables", async (int serverId) =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditSqlTables
                        .Where(x => x.SqlServerId == serverId)
                        .OrderBy(x => x.TableName)
                        .ToListAsync());
            });

            group.MapPost("/Tables", async (AuditSqlTable input) =>
            {
                using var context = new EnterpriseContext();

                context.AuditSqlTables.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/AuditSql/Tables/{input.Id}",
                    input);
            });

            group.MapPut("/Tables/{id:int}", async (int id, AuditSqlTable input) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditSqlTables.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.Entry(existing).CurrentValues.SetValues(input);

                await context.SaveChangesAsync();

                return Results.Ok(existing);
            });

            group.MapDelete("/Tables/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditSqlTables.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.AuditSqlTables.Remove(existing);

                await context.SaveChangesAsync();

                return Results.Ok();
            });

            #endregion

            #region AuditSqlFields

            group.MapGet("/Fields", async () =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditSqlFields
                        .OrderBy(x => x.ColumnName)
                        .ToListAsync());
            });

            group.MapGet("/Fields/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var item = await context.AuditSqlFields.FindAsync(id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            });

            group.MapGet("/Tables/{tableId:int}/Fields", async (int tableId) =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditSqlFields
                        .Where(x => x.SqlTableId == tableId)
                        .OrderBy(x => x.ColumnName)
                        .ToListAsync());
            });

            group.MapPost("/Fields", async (AuditSqlField input) =>
            {
                using var context = new EnterpriseContext();

                context.AuditSqlFields.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/AuditSql/Fields/{input.Id}",
                    input);
            });

            group.MapPut("/Fields/{id:int}", async (int id, AuditSqlField input) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditSqlFields.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.Entry(existing).CurrentValues.SetValues(input);

                await context.SaveChangesAsync();

                return Results.Ok(existing);
            });

            group.MapDelete("/Fields/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditSqlFields.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.AuditSqlFields.Remove(existing);

                await context.SaveChangesAsync();

                return Results.Ok();
            });

            #endregion

            #region AuditPrincipals

            group.MapGet("/Principals", async () =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditPrincipals
                        .OrderBy(x => x.UserName)
                        .ToListAsync());
            });

            group.MapGet("/Principals/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var item = await context.AuditPrincipals.FindAsync(id);

                return item == null
                    ? Results.NotFound()
                    : Results.Ok(item);
            });

            group.MapPost("/Principals", async (AuditPrincipal input) =>
            {
                using var context = new EnterpriseContext();

                context.AuditPrincipals.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/AuditSql/Principals/{input.Id}",
                    input);
            });

            group.MapPut("/Principals/{id:int}", async (int id, AuditPrincipal input) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditPrincipals.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.Entry(existing).CurrentValues.SetValues(input);

                await context.SaveChangesAsync();

                return Results.Ok(existing);
            });

            group.MapDelete("/Principals/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditPrincipals.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.AuditPrincipals.Remove(existing);

                await context.SaveChangesAsync();

                return Results.Ok();
            });

            #endregion

            #region AuditServerPermissions

            group.MapGet("/ServerPermissions", async () =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditServerPermissions.ToListAsync());
            });

            group.MapGet("/Principals/{principalId:int}/ServerPermissions", async (int principalId) =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditServerPermissions
                        .Where(x => x.PrincipalId == principalId)
                        .ToListAsync());
            });

            group.MapPost("/ServerPermissions", async (AuditServerPermission input) =>
            {
                using var context = new EnterpriseContext();

                context.AuditServerPermissions.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/AuditSql/ServerPermissions/{input.Id}",
                    input);
            });

            group.MapPut("/ServerPermissions/{id:int}", async (int id, AuditServerPermission input) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditServerPermissions.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.Entry(existing).CurrentValues.SetValues(input);

                await context.SaveChangesAsync();

                return Results.Ok(existing);
            });

            group.MapDelete("/ServerPermissions/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditServerPermissions.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.AuditServerPermissions.Remove(existing);

                await context.SaveChangesAsync();

                return Results.Ok();
            });

            #endregion

            #region AuditTablePermissions

            group.MapGet("/TablePermissions", async () =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditTablePermissions.ToListAsync());
            });

            group.MapGet("/Principals/{principalId:int}/TablePermissions", async (int principalId) =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditTablePermissions
                        .Where(x => x.PrincipalId == principalId)
                        .ToListAsync());
            });

            group.MapPost("/TablePermissions", async (AuditTablePermission input) =>
            {
                using var context = new EnterpriseContext();

                context.AuditTablePermissions.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/AuditSql/TablePermissions/{input.Id}",
                    input);
            });

            group.MapPut("/TablePermissions/{id:int}", async (int id, AuditTablePermission input) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditTablePermissions.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.Entry(existing).CurrentValues.SetValues(input);

                await context.SaveChangesAsync();

                return Results.Ok(existing);
            });

            group.MapDelete("/TablePermissions/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditTablePermissions.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.AuditTablePermissions.Remove(existing);

                await context.SaveChangesAsync();

                return Results.Ok();
            });

            #endregion

            #region AuditFieldPermissions

            group.MapGet("/FieldPermissions", async () =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditFieldPermissions.ToListAsync());
            });

            group.MapGet("/Principals/{principalId:int}/FieldPermissions", async (int principalId) =>
            {
                using var context = new EnterpriseContext();

                return Results.Ok(
                    await context.AuditFieldPermissions
                        .Where(x => x.PrincipalId == principalId)
                        .ToListAsync());
            });

            group.MapPost("/FieldPermissions", async (AuditFieldPermission input) =>
            {
                using var context = new EnterpriseContext();

                context.AuditFieldPermissions.Add(input);

                await context.SaveChangesAsync();

                return Results.Created(
                    $"/api/AuditSql/FieldPermissions/{input.Id}",
                    input);
            });

            group.MapPut("/FieldPermissions/{id:int}", async (int id, AuditFieldPermission input) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditFieldPermissions.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.Entry(existing).CurrentValues.SetValues(input);

                await context.SaveChangesAsync();

                return Results.Ok(existing);
            });

            group.MapDelete("/FieldPermissions/{id:int}", async (int id) =>
            {
                using var context = new EnterpriseContext();

                var existing = await context.AuditFieldPermissions.FindAsync(id);

                if (existing == null)
                    return Results.NotFound();

                context.AuditFieldPermissions.Remove(existing);

                await context.SaveChangesAsync();

                return Results.Ok();
            });

            #endregion
        }
    }
