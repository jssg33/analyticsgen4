using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace somecontrollers.Controllers;

public static class ApihostEndpoints
{
    public static void MapApihostEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Apihosts")
            .WithTags(nameof(Apihost));

        group.MapGet("/", () =>
        {
            using (var context = new EnterpriseContext())
            {
                return context.Apihosts.ToList();
            }
        })
        .WithName("GetAllApihosts")
        .WithOpenApi();

        group.MapGet("/{Id}", (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["Id"] ??
                httpContext.Request.RouteValues["id"];

            var Id = Convert.ToInt32(idObj);

            using (var context = new EnterpriseContext())
            {
                return Results.Ok(
                    context.Apihosts
                        .Where(x => x.Id == Id)
                        .ToList());
            }
        })
        .WithName("GetApihostById")
        .WithOpenApi();

        group.MapPost("/", async (Apihost input) =>
        {
            using (var context = new EnterpriseContext())
            {
                context.Apihosts.Add(input);

                await context.SaveChangesAsync();

                return TypedResults.Created(
                    $"Created ID:{input.Id}");
            }
        })
        .WithName("CreateApihost")
        .WithOpenApi();

        group.MapPut("/{id}",
        async (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["id"] ??
                httpContext.Request.RouteValues["Id"];

            var Id = Convert.ToInt32(idObj);

            var input =
                await httpContext.Request.ReadFromJsonAsync<Apihost>();

            using (var context = new EnterpriseContext())
            {
                var host =
                    context.Apihosts.FirstOrDefault(x => x.Id == Id);

                if (host == null)
                    return Results.NotFound();

                context.Apihosts.Attach(host);

host.ApiHostName = input!.ApiHostName;
host.ApiHostUrl = input.ApiHostUrl;
host.SwaggerUsername = input.SwaggerUsername;
host.SwaggerPassword = input.SwaggerPassword;
host.IsSecure = input.IsSecure;
host.LastAuditDate = input.LastAuditDate;
host.LastAuditId = input.LastAuditId;
host.Active = input.Active;

// New fields
host.AuditorId = input.AuditorId;
host.Family = input.Family;
host.IPv4Address = input.IPv4Address;
host.IPv6Address = input.IPv6Address;
host.HostName = input.HostName;
host.HasProxy = input.HasProxy;
host.ProxyEntranceV4 = input.ProxyEntranceV4;
host.ProxyEntranceV6 = input.ProxyEntranceV6;
host.ProxyType = input.ProxyType;
host.DatabaseFramework = input.DatabaseFramework;
host.DatabaseConnectionString = input.DatabaseConnectionString;
host.DatabaseType = input.DatabaseType;
host.AuditorName = input.AuditorName;
host.AuditorEmail = input.AuditorEmail;
host.AuditorEmployeeId = input.AuditorEmployeeId;
host.AuditorDepartment = input.AuditorDepartment;
host.BusinessUnitName = input.BusinessUnitName;
host.BusinessUnitOwnerId = input.BusinessUnitOwnerId;
host.PrimaryPort = input.PrimaryPort;
host.SecondaryPort = input.SecondaryPort;
host.SecondaryV4 = input.SecondaryV4;
host.SecondaryV6 = input.SecondaryV6;
host.FQDN = input.FQDN;
host.OSType = input.OSType;
host.FrameworkVersion = input.FrameworkVersion;
host.GroupId = input.GroupId;
host.GroupDescription = input.GroupDescription;
host.BuildingId = input.BuildingId;
host.BuildingName = input.BuildingName;
host.TechContactName = input.TechContactName;
host.TechContactEmail = input.TechContactEmail;
host.TechContactFax = input.TechContactFax;
host.TechContactPhone = input.TechContactPhone;
host.SecurityEmail = input.SecurityEmail;
host.SecurityPhone = input.SecurityPhone;
host.DBAId = input.DBAId;
host.DBAName = input.DBAName;
host.HashType = input.HashType;
host.SslSupported = input.SslSupported;
host.HttpSupported = input.HttpSupported;
host.HttpPortV4 = input.HttpPortV4;
host.HttpPortV6 = input.HttpPortV6;
host.CorsPath = input.CorsPath;
host.AllowedRanges = input.AllowedRanges;
host.DeniedRanges = input.DeniedRanges;
host.IsActive = input.IsActive;
host.Environment = input.Environment;
host.ApplicationName = input.ApplicationName;
host.SourceRepository = input.SourceRepository;
host.AzureSubscriptionId = input.AzureSubscriptionId;
host.AzureResourceGroup = input.AzureResourceGroup;
host.SecurityClassification = input.SecurityClassification;
//New Fields to Allow Audit Information to Be Injected Into Inventory
host.InventoryId = input.InventoryId;
host.ServicePath = input.ServicePath;
host.ServiceId = input.ServiceId;
host.ServiceName = input.ServiceName;

                await context.SaveChangesAsync();

                return Results.Accepted(
                    $"Updated ID:{host.Id}");
            }
        })
        .WithName("UpdateApihost")
        .WithOpenApi();

        group.MapDelete("/{Id}",
        async (HttpContext httpContext) =>
        {
            var idObj =
                httpContext.Request.RouteValues["Id"] ??
                httpContext.Request.RouteValues["id"];

            var Id = Convert.ToInt32(idObj);

            using (var context = new EnterpriseContext())
            {
                var host =
                    context.Apihosts.FirstOrDefault(x => x.Id == Id);

                if (host == null)
                    return Results.NotFound();

                context.Apihosts.Remove(host);

                await context.SaveChangesAsync();

                return Results.Ok();
            }
        })
        .WithName("DeleteApihost")
        .WithOpenApi();
    }
}
