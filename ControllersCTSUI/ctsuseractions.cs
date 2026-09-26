/*using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Enterprise.Models;
using EnterpriseServices;
using Microsoft.Extensions.WebEncoders.Testing;
namespace Enterprise.Controllers;


//GENERAL COMMENTS
//USER ACTIONS IN OUR SYSTEM ARE MINIMIZED TO GET ACTIONS PRIMARILY BECAUSE DELETE AND UPDATE ACTIONS CAN BE DONE BY RECORDID WHICH ARE NORMAL SQL OPERATIONS AGAINST THE PRIMARY INDEX.
//SO A GET BY USER ID, WILL RETURN A SPECIFIC RECORD OR SET OF RECORDS WHICH INCLUDES THE PRIMARY INDEX(int id) IN THE RETURNED TUPLE/TUPLES, AND SUBSEQUENT ACTIONS CAN BE DONE AGAINST THE PRIMARY INDEX.
//IN OTHER WORDS USER IS A SECONDARY INDEX, NOT A PRIMARY ONE IN OUR ARCHITECTURE. 
//A COLLEAGUE OF OURS HOWEVER SHOWED US THAT THE POST FUNCTION AGAINST A NON-DATABASE IN-MEMORY STRUCTURE WAS VERY HELPFUL IN PASSING PARAMETERS TO FUNCTIONS WHICH REQUIRED MORE THAN ONE PARAMETER.
//THE REASON IS THAT REST ENDPOINTS ONLY SUPPORT ONE KEY BY DEFAULT /api/endpoint/{Indexid} FOR EXAMPLE....BUT IF YOU NEED TWO....LIKE IN AUTH ACTIONS, USER/PARK COMBINATIONS, OR CONTROLLER ACTIONS YOU NEED MORE THAN ONE.
//TO ACHIEVE USER ACTIONS AND COMPLEX FUNCTIONS ACROSS THE INTERNET YOU NEED TO USE POST ACTIONS WITH ALL THE PARAMETERS WRAPPED IN JSON FORM (THANKS KALEB... HES KINDA SMART)...AND THAT REQUIRES A POST WITH SPECIAL RETURN
//TYPES... AND YOU DONT HAVE TO WRITE THESE OUT TO DATABASES USING C#.


public static class AllCGUserEndpoints
{
    
    public static void MapAllCGUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Userstuff").WithTags(nameof(Template));
        EnterpriseServices.Globals.ControllerAPIName = "TemplateAPI";
        EnterpriseServices.Globals.ControllerAPINumber = "999";
        

        //[HttpGet]
        group.MapGet("/cartmaster", (int userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_CARTMASTER_BY_USERID", 1, "Test", "Test"); 
                return context.CartMasters.Where(m => m.Id == userid).ToList();
            }
        })
        .WithName("CGGetCartMasterByUserIdInt")
        .WithOpenApi();


        //[HttpGet]
        group.MapGet("/cart", (int userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_CARTS_BY_USERID", 1, "Test", "Test"); 
                return context.Carts.Where(m => m.Userid == userid).ToList();
            }
        })
        .WithName("CGGetCartsByUserIdInt")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/cartitems", (int userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_CARTITEM_BY_USERID", 1, "Test", "Test"); 
                return context.Cartitems.Where(m => m.Userid == userid).ToList();
            }
        })
        .WithName("CGGetCartItemsByUserIdInt")
        .WithOpenApi();


        //[HttpGet]
        group.MapGet("/payments", (int userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_PAYMENTS_BY_USERID", 1, "Test", "Test"); 
                return context.Payments.Where(m => m.Userid == userid).ToList();
            }
        })
        .WithName("CGGetPaymentsByUserIdInt")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/cards", (int userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_CREDITCARDS_BY_USERID", 1, "Test", "Test"); 
                return context.Cards.Where(m => m.Userid == userid).ToList();
            }
        })
        .WithName("CGGetCardsByUserIdInt")
        .WithOpenApi();


        //[HttpGet]
        group.MapGet("/reservations", (int userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_RESERVATIONS_BY_USERID", 1, "Test", "Test"); 
                return context.Bookings.Where(m => m.Userid == userid).ToList();
            }
        })
        .WithName("GCGetReservationsByUserIdInt")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/user", (int userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_user_BY_USERID_int", 1, "Test", "Test"); 
                return context.Users.Where(m => m.Userid == userid).ToList();
            }
        })
        .WithName("GCGetUserId")
        .WithOpenApi();

      //[HttpGet]
        group.MapGet("/userprofile", (int userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_profile_BY_USERID_int", 1, "Test", "Test"); 
                return context.Userprofiles.Where(m => m.Userid == userid).ToList();
            }
        })
        .WithName("GCGetUserProfileId")
        .WithOpenApi();



	//SUPPORTING CHARACTER BASED USER MODELS

        //[HttpGet]
        group.MapGet("/cartmasterstring", (string userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_CARTMASTER_BY_USERID", 1, "Test", "Test"); 
                return context.CartMasters.Where(m => m.Useridstring == userid).ToList();
            }
        })
        .WithName("CGGetCartMasterByUserIdString")
        .WithOpenApi();


        //[HttpGet]
        group.MapGet("/cartstring", (string userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_CARTS_BY_USERID", 1, "Test", "Test"); 
                return context.Carts.Where(m => m.Uid == userid).ToList();
            }
        })
        .WithName("CGGetCartsByUserIdString")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/cartitemsstring", (string userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_CARTITEM_BY_USERID", 1, "Test", "Test"); 
                return context.Cartitems.Where(m => m.Useridstring == userid).ToList();
            }
        })
        .WithName("CGGetCartItemsByUserIdString")
        .WithOpenApi();


        //[HttpGet]
        group.MapGet("/paymentsstring", (string userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_PAYMENTS_BY_USERID", 1, "Test", "Test"); 
                return context.Payments.Where(m => m.Useridasstring == userid).ToList();
            }
        })
        .WithName("CGGetPaymentsByUserIdString")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/cardsstring", (string userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_CREDITCARDS_BY_USERID", 1, "Test", "Test"); 
                return context.Cards.Where(m => m.Uid == userid).ToList();
            }
        })
        .WithName("CGGetCardsByUserIdString")
        .WithOpenApi();


        //[HttpGet]
        group.MapGet("/reservationsstring", (string userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_RESERVATIONS_BY_USERID", 1, "Test", "Test"); 
                return context.Bookings.Where(m => m.Uid == userid).ToList();
            }
        })
        .WithName("GCGetReservationsByUserIdString")
        .WithOpenApi();

        //[HttpGet]
        group.MapGet("/userstring", (string userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_USER_BY_USERID_STRING", 1, "Test", "Test"); 
                return context.Users.Where(m => m.Uidstring == userid).ToList();
            }
        })
        .WithName("GCGetUserIdString")
        .WithOpenApi();

      //[HttpGet]
        group.MapGet("/userprofilestring", (string userid) =>
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(EnterpriseServices.Globals.ControllerAPIName, EnterpriseServices.Globals.ControllerAPINumber, "GET_PROFILE_BY_USERID_STRING", 1, "Test", "Test"); 
                return context.Userprofiles.Where(m => m.Useridstring == userid).ToList();
            }
        })
        .WithName("GCGetUserProfileIdString")
        .WithOpenApi();

    }
}

*/