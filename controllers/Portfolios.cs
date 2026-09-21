using System;
using System.Linq;
using Enterprise.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

namespace somecontrollers.Controllers;

public static class PortfolioEndpoints
{
public static void MapPortfolioEndpoints(this IEndpointRouteBuilder routes)
{
var group = routes.MapGroup("/api/Portfolios").WithTags(nameof(Portfolio));
group.MapGet("/", () =>
{
using (var context = new EnterpriseContext())
{
return context.Portfolios.ToList();
}
})
.WithName("GetAllPortfolios")
.WithOpenApi();
 

group.MapGet("/{id}", (int id) =>
{
using (var context = new EnterpriseContext())
{
return context.Portfolios

.Where(m => m.Id == id)
.ToList();
}
})
.WithName("GetPortfolioById")
.WithOpenApi();

group.MapGet("/user/{userid}", (string userid) =>
{
using (var context = new EnterpriseContext())
{
return context.Portfolios
.Where(m => m.UserId == userid)
.ToList();
}
})
.WithName("GetPortfolioByUserId")
.WithOpenApi();

group.MapPut("/{id}", async (int id, Portfolio input) =>
{
using (var context = new EnterpriseContext())
{
Portfolio[] portfolios = context.Portfolios
.Where(m => m.Id == id)
.ToArray();
context.Portfolios.Attach(portfolios[0]);
portfolios[0].UserId = input.UserId;
portfolios[0].PortfolioTargetInvestment = input.PortfolioTargetInvestment;
await context.SaveChangesAsync();
return TypedResults.Accepted("Updated ID:" + input.Id);
}
})
.WithName("UpdatePortfolio")
.WithOpenApi();

group.MapPost("/", async (Portfolio input) =>
{
using (var context = new EnterpriseContext())
{
context.Portfolios.Add(input);
await context.SaveChangesAsync();
return TypedResults.Created("Created ID:" + input.Id);
}
})
.WithName("CreatePortfolio")
.WithOpenApi();

group.MapDelete("/{id}", async (int id) =>
{
using (var context = new EnterpriseContext())
{
Portfolio[] portfolios = context.Portfolios
.Where(m => m.Id == id)
.ToArray();
context.Portfolios.Attach(portfolios[0]);
context.Portfolios.Remove(portfolios[0]);
await context.SaveChangesAsync();
}
})
.WithName("DeletePortfolio")
.WithOpenApi();
}

}
