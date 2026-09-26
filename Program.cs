using somecontrollers.Controllers;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Enterprise.Models;
using Enterprise.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using EnterpriseServices;

var builder = WebApplication.CreateBuilder(args);

// AUTHENTICATION + AZURE AD
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        builder.Configuration.GetSection("AzureAd"));

// AUTHORIZATION
builder.Services.AddAuthorization();

// CONTROLLERS
builder.Services.AddControllers();

// SWAGGER / OPENAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.OrderActionsBy(api =>
        $"{api.RelativePath}_{api.HttpMethod}");
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// CUSTOM SERVICES
builder.Services.AddScoped<ServiceCipherSupportsService>();

var app = builder.Build();

// CORS
app.UseCors("AllowAll");

// SWAGGER
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "My API V4");
});

// AUTHENTICATION
app.UseAuthentication();

// AUTHORIZATION
app.UseAuthorization();

// MVC CONTROLLERS
app.MapControllers();

// MINIMAL API ENDPOINTS
app.MapAdminlogsEndpoints();
app.MapAllstockEndpoints();
//09/21/2026 - Added API Auditor
app.MapApiAuditEndpoints();
app.MapApihostEndpoints();
app.MapApiAuditResultEndpoints();
app.MapApiAuditExceptionEndpoints();

app.MapApilogEndpoints();
app.MapAuthEndpoints();
app.MapCipherSupportEndpoints();
app.MapCockyCipherBlockEndpoints();
app.MapCustomerEndpoints();
app.MapKeyServerEndpoints();
app.MapLearndetailEndpoints();
app.MapLearnLogEndpoints();
app.MapPortfolioEndpoints();
app.MapSectorssummaryEndpoints();
app.MapSessionlogEndpoints();
app.MapSuperuserlogEndpoints();
app.MapSystemStocksEndpoints();
app.MapTradeOrdersEndpoints();
app.MapTraderEndpoints();
app.MapUseractionEndpoints();
app.MapUserEndpoints();
app.MapUsergroupsEndpoints();
app.MapUserhelpEndpoints();
app.MapUserLocationEndpoints();
app.MapUserlogEndpoints();
app.MapUserNoticeEndpoints();
app.MapUserprofileEndpoints();
app.MapUsersessionEndpoints();
app.MapApiHostExceptionEndpoints();
app.MapApplicationEndpoints();
app.MapApplicationApiEndpoints();

//CONVERSIONS TO MAP REDUCED FORM
app.MapLunaLogEndpoints();
app.MapSysLogEndpoints();
app.MapUserProfileLogEndpoints();
app.MapApiInterfacesAuditEndpoints();
app.MapUserDownloadLogEndpoints();

//app.Run();
//app.Run();
//THIS ROUTINE RUNS A PASSWORD HASHER AGAINST THE CURRENT USER TABLE.
//IT WILL REBUILD THE PASSWORDS ALSO USING A RANDOM HASHER USING BCRYPT
//THE SAME PASSWORD WILL GENERATE A UNIQUE STRING EVERY TIME.
//AUTH WILL FAIL WITHOUT THE BCRYPT SO EVEN HAVING THE PLAIN PASSWORD IS NO HELP.
//COMMENTED OUT AS IT SHOULD ONLY BE RUN WITH ADMINISTRATOR PERMISSION.
//WE DO NEED TO CONSIDER WHETHER THE /API/USER GET NEEDS TO BE PRESENT AND OR PERMISSIONS ON USERMANAGER.

//var myPasswords = new MyPasswords();
//await MyPasswords.HashAllUserPasswordsAsync();

if (builder.Environment.IsDevelopment()) { await RunCliAsync(); } 
await app.RunAsync();


static async Task RunCliAsync()
{
    Console.WriteLine("=== Dirtbike System Console CLI ===");
    Console.WriteLine("Type 'help' for commands, 'exit' to quit.");

    string? input;
    do
    {
        Console.Write("> ");
        input = Console.ReadLine()?.Trim().ToLower();

        switch (input)
        {
            case "help":
                Console.WriteLine("Available commands:");
                Console.WriteLine("  schema     - Dump DB schema");
                Console.WriteLine("  ncparks    - Process NC parks from DATA directory");
                Console.WriteLine("  vaparks    - Process VA parks from DATA directory");
                Console.WriteLine("  allparks   - Process ALL parks in IOQUEUE via SQL");
                Console.WriteLine("  files      - Show file list");
                Console.WriteLine("  zerocarts  - Remove zero carts for a user");
                Console.WriteLine("  avg        - Update avg rating for one park");
                Console.WriteLine("  avgall     - Update avg rating for first 500 parks");
                Console.WriteLine(" initdata - Load initial.sql (parks, users, reviews)");
                Console.WriteLine("  exit       - Quit CLI");
                break;

            case "schema":
                EnterpriseServices.SystemCLISupport.DumpSchema();
                break;

            case "ncparks":
                SystemCLISupport.ProcessNCParks();
                break;

            case "vaparks":
                SystemCLISupport.ProcessVAParks();
                break;

            case "allparks":
                SystemCLISupport.ProcessAllParks();
                break;

            case "files":
                SystemCLISupport.ShowFileList();
                break;

            case "initdata": 
                Console.WriteLine("Running initial.sql..."); 
                EnterpriseServices.DatabaseTools.LoadInitData(); 
                break;

            case "zerocarts":
                Console.Write("Enter user ID: ");
                if (int.TryParse(Console.ReadLine(), out int userId))
                    SystemCLISupport.RemoveZeroCarts(userId);
                else
                    Console.WriteLine("Invalid user ID.");
                break;

            case "avg":
                Console.Write("Enter park ID: ");
                if (int.TryParse(Console.ReadLine(), out int parkId))
                    SystemCLISupport.UpdateParkAvg(parkId);
                else
                    Console.WriteLine("Invalid park ID.");
                break;

            case "avgall":
                SystemCLISupport.UpdateAllParkAvgs();
                break;

            case "exit":
                Console.WriteLine("Exiting CLI...");
                break;

            default:
                if (!string.IsNullOrWhiteSpace(input))
                    Console.WriteLine($"Unknown command: {input}");
                break;
        }

    } while (input != "exit");

    await Task.CompletedTask;
}



