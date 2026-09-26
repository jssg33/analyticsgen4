/*using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;
using Enterprise.DTOs;
using EnterpriseServices;

namespace Enterprise.Controllers
{
    public static class MapCGParksEndpoints
    {
        public static void MapCGUIParksEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/CGParks").WithTags(nameof(CGPARKS));
            EnterpriseServices.Globals.ControllerAPIName = "CGParksAPI";
            EnterpriseServices.Globals.ControllerAPINumber = "003";

            // GET all CG parks
            group.MapGet("/", () =>
            {
                var service = new CGPARKSService();

                EnterpriseServices.ApiLogger.logapi(
                    EnterpriseServices.Globals.ControllerAPIName,
                    EnterpriseServices.Globals.ControllerAPINumber,
                    "CGGETALL", 1, "Fetch", "All CG Parks");

                return service.GetAllParks();
            })
            .WithName("CGGetAllParks")
            .WithOpenApi();

            // GET CG park by UUID
            group.MapGet("/{uuid}", (string uuid) =>
            {
                var service = new CGPARKSService();

                EnterpriseServices.ApiLogger.logapi(
                    EnterpriseServices.Globals.ControllerAPIName,
                    EnterpriseServices.Globals.ControllerAPINumber,
                    "CGGETWITHUUID", 1, "Fetch", $"CG Park {uuid}");

                var dto = service.GetParkByUuid(uuid);
                return dto == null ? Results.NotFound() : Results.Ok(dto);
            })
            .WithName("CGGetParkByUuid")
            .WithOpenApi();

            // POST new CG park
            group.MapPost("/", (Park input) =>
            {
                var service = new CGPARKSService();
                var created = service.CreatePark(input);

                EnterpriseServices.ApiLogger.logapi(
                    EnterpriseServices.Globals.ControllerAPIName,
                    EnterpriseServices.Globals.ControllerAPINumber,
                    "CGNEWRECORD", 1, "Create", $"CG Park {created.Id}");

                return TypedResults.Created($"/api/CGParks/{created.Id}", created);
            })
            .WithName("CGCreatePark")
            .WithOpenApi();

            // PUT update CG park by UUID
            group.MapPut("/{uuid}", (string uuid, Park input) =>
            {
                var service = new CGPARKSService();
                var updated = service.UpdatePark(uuid, input);

                EnterpriseServices.ApiLogger.logapi(
                    EnterpriseServices.Globals.ControllerAPIName,
                    EnterpriseServices.Globals.ControllerAPINumber,
                    "CGUPDATE", 1, "Update", $"CG Park {uuid}");

                return updated ? Results.Accepted($"Updated CG Park UUID: {uuid}") : Results.NotFound();
            })
            .WithName("CGUpdatePark")
            .WithOpenApi();

            // DELETE CG park by UUID
            group.MapDelete("/{uuid}", (string uuid) =>
            {
                var service = new CGPARKSService();
                var deleted = service.DeletePark(uuid);

                EnterpriseServices.ApiLogger.logapi(
                    EnterpriseServices.Globals.ControllerAPIName,
                    EnterpriseServices.Globals.ControllerAPINumber,
                    "CGDELETE", 1, "Delete", $"CG Park {uuid}");

                return deleted ? Results.Ok($"Deleted CG Park UUID: {uuid}") : Results.NotFound();
            })
            .WithName("CGDeletePark")
            .WithOpenApi();
        }
    }
}
*/