using EnterpriseServices;
using Microsoft.AspNetCore.Mvc;
using Enterprise.Models;
using System.Linq;
using Enterpriseservices;

namespace EnterpriseControllers;

// THIS IS A FANCY GRID CONTROLLER.
// IT PROMPTS THE USER FOR AN ACTION WHICH IS AN INTEGER.
// IT RUNS FUNCTIONS ON THE CONSOLE AND RETURNS DATA AS NECESSARY.
// OPTIONS 1, 2, 3, AND 4 IMPACT PROCESSING OF PARKS FROM THE COMMAND LINE.
// OPTION 5 DUMPS THE VALUE OF FILES IN THE QUEUE FOR PROCESSING.
// OPTION 6 REMOVES ZERO CARTS FOR A USER.
// OPTIONS 7 AND 8 IMPACT PROCESSING OF PARK REVIEWS.
// OPTION 9 POLLS BACKEND WEB SERVERS FOR HTTP/HTTPS SUPPORT.
// OPTION 10 RETURNS COMPILED INTERFACES AND THEIR METHODS.

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
GET /SystemConsole/10
*/

[ApiController]
[Route("[controller]")]
public class SystemConsoleController : ControllerBase
{
    [HttpGet("{option}")]
    public IActionResult GetSystemInfo(int option, [FromQuery] int value)
    {
        EnterpriseServices.Globals.ControllerAPIName = "ActionsController";
        EnterpriseServices.Globals.ControllerAPINumber = "001";

        switch (option)
        {
            case 1:
                SystemCLISupport.DumpSchema();
                return Ok("Schema dumped.");

            case 2:
                SystemCLISupport.ProcessNCParks();
                return Ok("NC parks processed.");

            case 3:
                SystemCLISupport.ProcessVAParks();
                return Ok("VA parks processed.");

            case 4:
                SystemCLISupport.ProcessAllParks();
                return Ok("All parks processed.");

            case 5:
                SystemCLISupport.ShowFileList();
                return Ok("File list displayed.");

            case 6:
                // using var context = new EnterpriseContext();
                // var zeroCartService = new ZeroCartService(context);
                // var result = zeroCartService.ZeroCartUpdate(value.ToString());
                // return Ok(result);

                return Ok("Option 6 is currently disabled.");

            case 7:
                // using var context = new EnterpriseContext();
                // var ratingService = new ParkRatingService(context);
                // var result = ratingService.UpdateAverageParkRating(value);
                // return Ok(result);

                return Ok("Option 7 is currently disabled.");

            //THIS API POWERS THE COCKYAUDITOR AND COCKYANALYTICS. THE AUDITOR TOOL CAN IMPORT THE ENTIRE TEXT FILE.
            case 8:
                return Ok(SqlAzureSecurityAudit.RunAudit());

            case 9:
                return Ok(BackendWebServerPoller.PollBackendServers());
            
            case 10:
                return Ok(GetRegisteredInterfaces());

            default:
                return BadRequest("Invalid option.");
        }
    }

    private static object GetRegisteredInterfaces()
    {
        return typeof(IApilogService02)
            .Assembly
            .GetTypes()
            .Where(t =>
                t.IsInterface &&
                t.Namespace == "Enterpriseservices")
            .OrderBy(t => t.Name)
            .Select(t => new
            {
                InterfaceName = t.Name,
                MethodCount = t.GetMethods().Length,
                Methods = t.GetMethods()
                    .Select(m => m.Name)
                    .OrderBy(m => m)
                    .ToList()
            })
            .ToList();
    }
}

public class ControlBlock
{
    public string? somestring1 { get; set; }
    public string? somestring2 { get; set; }
    public required int i1 { get; set; }
    public required int i2 { get; set; }
}
