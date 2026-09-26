/*
using EnterpriseServices;
using Enterprise.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.Controllers
{
    public static class MapCGCartEndpoints
    {
        public static void MapCGUICartEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/CGCart").WithTags("CGCart");
            EnterpriseServices.Globals.ControllerAPIName = "CGCartAPI";
            EnterpriseServices.Globals.ControllerAPINumber = "005";

            // CREATE
            group.MapPost("/", (CGCompletedCartDto dto) =>
            {
                var service = new CGCartService();
                var cart = service.CreateCart(dto);

                //EnterpriseServices.ApiLogger.logapi("CGCartAPI", "005", "CGCREATE", 1, "Create", $"Cart {dto.Uid}"); //CHECK THIS LOG
                return cart == null ? Results.BadRequest("User not found") : Results.Created($"/api/CGCart", cart); //CHECK THIS BEFORE SUBMISSION.
            })
            .WithName("CGCreateCart")
            .WithOpenApi();

            /*
            // READ all carts for a user
            group.MapGet("/user/{userid}", (int userid) =>
            {
                var service = new CGCartService();
                var carts = service.GetCartsByUserId(userid);

                EnterpriseServices.ApiLogger.logapi("CGCartAPI", "005", "CGGETBYUSER", 1, "Fetch", $"Carts for User {userid}");
                return carts.Count == 0 ? Results.NotFound() : Results.Ok(carts);
            })
            .WithName("CGGetCartsByUserId")
            .WithOpenApi();

            // READ single cart
            group.MapGet("/{cartId}", (int cartId) =>
            {
                var service = new CGCartService();
                var cart = service.GetCartById(cartId);

                EnterpriseServices.ApiLogger.logapi("CGCartAPI", "005", "CGGETBYID", 1, "Fetch", $"Cart {cartId}");
                return cart == null ? Results.NotFound() : Results.Ok(cart);
            })
            .WithName("CGGetCartById")
            .WithOpenApi();

            // UPDATE
            group.MapPut("/{cartId}", (int cartId, CGCompletedCartDto dto) =>
            {
                var service = new CGCartService();
                var updated = service.UpdateCart(cartId, dto);

                EnterpriseServices.ApiLogger.logapi("CGCartAPI", "005", "CGUPDATE", 1, "Update", $"Cart {cartId}");
                return updated ? Results.Accepted($"Updated Cart {cartId}") : Results.NotFound();
            })
            .WithName("CGUpdateCart")
            .WithOpenApi();

            // DELETE
            group.MapDelete("/{cartId}", (int cartId) =>
            {
                var service = new CGCartService();
                var deleted = service.DeleteCart(cartId);

                EnterpriseServices.ApiLogger.logapi("CGCartAPI", "005", "CGDELETE", 1, "Delete", $"Cart {cartId}");
                return deleted ? Results.Ok($"Deleted Cart {cartId}") : Results.NotFound();
            })
            .WithName("CGDeleteCart")
            .WithOpenApi();
        }
    }
}
*/