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

app.Run();
