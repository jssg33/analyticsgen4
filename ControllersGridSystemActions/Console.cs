using EnterpriseServices; // <-- bring in FileIO
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Enterprise.Models;
using System.Reflection;

namespace EnterpriseControllers;


//THIS IS A FANCY GRID CONTROLLER.
//IT PROMPTS THE USER FOR AN ACTION WHICH IS AN INTEGER.
//IT RUNS FUNCTIONS ON THE CONSOLE, AND RETURNS DATA AS NECESSARY.
//OPTIONS 1,2,3,4 IMPACT PROCESSING OF PARKS FROM THE COMMAND LINE
//OPTIONS 5 DUMPS THE VALUE OF FILES IN THE QUEUE FOR PROCESSING.
//OPTIONS 6 REMOVES ZERO CARTS FOR A USER WHICH IS REQUIRED IN SOME SCREENS TO REVIEW PENDING CART ITEMS ON PARKS UI.
//OPTIONS 7/8 IMPACT THE PROCESSING OF PARK REVIEWS WHICH MAY NEED TO BE TRIGGERED FROM THE UI.

//12-26 UPDATED CONTROLLER TO ADD ALL CLI COMMANDS - EXAMPLES ARE BELOW
/*
GET /SystemConsole/1
GET /SystemConsole/2
GET /SystemConsole/3
GET /SystemConsole/4
GET /SystemConsole/5
GET /SystemConsole/6?value=123
GET /SystemConsole/7?value=55
GET /SystemConsole/8
GET /SystemConsole/9
*/

[ApiController]
[Route("[controller]")]
public class SystemConsoleController : ControllerBase
{
    private readonly EnterpriseContext _context;

    public SystemConsoleController(EnterpriseContext context)
    {
        _context = context;
    }

    [HttpGet("{option}")]
    public IActionResult GetSystemInfo(int option, [FromQuery] int value)
    {
        EnterpriseServices.Globals.ControllerAPIName = "ActionsController";
        EnterpriseServices.Globals.ControllerAPINumber = "001";

        switch (option)
        {
            case 1:
                EnterpriseServices.SystemCLISupport.DumpSchema();
                return Ok("Schema dumped");

            case 2:
                EnterpriseServices.SystemCLISupport.ProcessNCParks();
                return Ok("NC parks processed");

            case 3:
                EnterpriseServices.SystemCLISupport.ProcessVAParks();
                return Ok("VA parks processed");

            case 4:
                EnterpriseServices.SystemCLISupport.ProcessAllParks();
                return Ok("All parks processed");

            case 5:
                EnterpriseServices.SystemCLISupport.ShowFileList();
                return Ok("File list displayed");

            case 6:
                //var zeroCartService = new ZeroCartService(_context);
                //var zeroResult = zeroCartService.ZeroCartUpdate(value.ToString());
                //return Ok($"Zero carts removed for user {value}: {zeroResult}");

            case 7:
                //var ratingService = new ParkRatingService(_context);
                //var avgResult = ratingService.UpdateAverageParkRating(value);
                //return Ok($"Average updated for park {value}: {avgResult}");

            case 8:
                //var ratingServiceAll = new ParkRatingService(_context);
                //var avgAllResult = ratingServiceAll.UpdateAverageRatingsForFirst500();
                //return Ok($"Average updated for first 500 parks: {avgAllResult}");

            case 9:
                EnterpriseServices.DatabaseTools.LoadInitData();
                return Ok("Initial SQL data loaded");

            default:
                return BadRequest("Invalid option");
        }
    }
}


public class ControlBlock 
{ 
public string? somestring1 { get; set; } 
public string? somestring2 { get; set; } 
public required int i1 { get; set; } 
public required int i2 { get; set; }  
}



