using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;

namespace somecontrollers.Controllers;

public static class ApiAuditEndpoints
{
    public static void MapApiAuditEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Apiaudit")
            .WithTags(nameof(Apiaudit));

        // GET ALL
        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Apiaudits.ToList();
            }
        })
        .WithName("GetAllApiaudits")
        .WithOpenApi();

        // GET BY ID
        group.MapGet("/{Id}", (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["Id"]
                ?? httpContext.Request.RouteValues["id"];

            var Id = Convert.ToInt32(idObj);

            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.Apiaudits
                        .Where(x => x.Id == Id)
                        .ToList());
            }
        })
        .WithName("GetApiauditById")
        .WithOpenApi();

        // CREATE
        group.MapPost("/", async (Apiaudit input) =>
        {
            using (var context = new EnterpriseContext())
            {
                context.Apiaudits.Add(input);

                await context.SaveChangesAsync();

                return TypedResults.Created(
                    $"Created ID:{input.Id}");
            }
        })
        .WithName("CreateApiaudit")
        .WithOpenApi();

        // UPDATE
        group.MapPut("/{id}",
        async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["id"]
                ?? httpContext.Request.RouteValues["Id"];

            var Id = Convert.ToInt32(idObj);

            var input =
                await httpContext.Request.ReadFromJsonAsync<Apiaudit>();

            using (var context = new EnterpriseContext())
            {
                var audit =
                    context.Apiaudits.FirstOrDefault(x => x.Id == Id);

                if (audit == null)
                    return Results.NotFound();

                context.Apiaudits.Attach(audit);

                audit.AuditorId = input!.AuditorId;
                audit.Description = input.Description;
                audit.Family = input.Family;
                audit.Url = input.Url;
                audit.ApiRoot = input.ApiRoot;
                audit.Type = input.Type;
                audit.ServicesAttached = input.ServicesAttached;
                audit.ServicesJson = input.ServicesJson;
                audit.IPv4Address = input.IPv4Address;
                audit.IPv6Address = input.IPv6Address;
                audit.HostName = input.HostName;
                audit.HasProxy = input.HasProxy;
                audit.ProxyEntranceV4 = input.ProxyEntranceV4;
                audit.ProxyEntranceV6 = input.ProxyEntranceV6;
                audit.ProxyType = input.ProxyType;
                audit.DatabaseFramework = input.DatabaseFramework;
                audit.DatabaseConnectionString = input.DatabaseConnectionString;
                audit.DatabaseType = input.DatabaseType;
                audit.HasAuthEnabled = input.HasAuthEnabled;
                audit.AuthType = input.AuthType;
                audit.AuditorName = input.AuditorName;
                audit.AuditorEmail = input.AuditorEmail;
                audit.AuditorEmployeeId = input.AuditorEmployeeId;
                audit.AuditorDepartment = input.AuditorDepartment;
                audit.BusinessUnitName = input.BusinessUnitName;
                audit.BusinessUnitOwnerId = input.BusinessUnitOwnerId;
                audit.PrimaryPort = input.PrimaryPort;
                audit.SecondaryPort = input.SecondaryPort;
                audit.SecondaryV4 = input.SecondaryV4;
                audit.SecondaryV6 = input.SecondaryV6;
                audit.FQDN = input.FQDN;
                audit.OSType = input.OSType;
                audit.FrameworkVersion = input.FrameworkVersion;
                audit.GroupId = input.GroupId;
                audit.GroupDescription = input.GroupDescription;
                audit.BuildingId = input.BuildingId;
                audit.BuildingName = input.BuildingName;
                audit.TechContactName = input.TechContactName;
                audit.TechContactEmail = input.TechContactEmail;
                audit.TechContactFax = input.TechContactFax;
                audit.TechContactPhone = input.TechContactPhone;
                audit.SecurityEmail = input.SecurityEmail;
                audit.SecurityPhone = input.SecurityPhone;
                audit.DBAId = input.DBAId;
                audit.DBAName = input.DBAName;
                audit.HashType = input.HashType;
                audit.EndpointType = input.EndpointType;
                audit.SslSupported = input.SslSupported;
                audit.HttpSupported = input.HttpSupported;
                audit.HttpPortV4 = input.HttpPortV4;
                audit.HttpPortV6 = input.HttpPortV6;
                audit.CorsPath = input.CorsPath;
                audit.AllowedRanges = input.AllowedRanges;
                audit.DeniedRanges = input.DeniedRanges;

                audit.ModifiedDate = DateTime.UtcNow;

                audit.LastAuditDate = input.LastAuditDate;
                audit.IsActive = input.IsActive;
                audit.Environment = input.Environment;
                audit.ApplicationName = input.ApplicationName;
                audit.SourceRepository = input.SourceRepository;
                audit.AzureSubscriptionId = input.AzureSubscriptionId;
                audit.AzureResourceGroup = input.AzureResourceGroup;
                audit.ApiHostId = input.ApiHostId;
                audit.AuditResultId = input.AuditResultId;
                audit.EndpointSecure = input.EndpointSecure;
                audit.SwaggerOperationId = input.SwaggerOperationId;
                audit.SwaggerVersion = input.SwaggerVersion;
                audit.Notes = input.Notes;
                audit.SecurityClassification = input.SecurityClassification;

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Updated ID:{audit.Id}");
            }
        })
        .WithName("UpdateApiaudit")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{Id}",
        async (HttpContext httpContext) =>
        {
            var idObj = httpContext.Request.RouteValues["Id"]
                ?? httpContext.Request.RouteValues["id"];

            var Id = Convert.ToInt32(idObj);

            using (var context = new EnterpriseContext())
            {
                var audit =
                    context.Apiaudits.FirstOrDefault(x => x.Id == Id);

                if (audit == null)
                    return Results.NotFound();

                context.Apiaudits.Remove(audit);

                await context.SaveChangesAsync();

                return Results.Ok();
            }
        })
        .WithName("DeleteApiaudit")
        .WithOpenApi();
    }
}
