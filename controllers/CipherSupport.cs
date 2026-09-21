using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using Enterprise.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
namespace somecontrollers.Controllers;


public static class CipherSupportEndpoints
{
	public static void MapCipherSupportEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/CipherSupport").WithTags(nameof(CipherSupport));

		// GET all
		group.MapGet("/", () =>
		{
			using (var context = new EnterpriseContext())
			{
				return context.CipherSupports.ToList();
			}
		})
		.WithName("GetAllCipherSupports")
		.WithOpenApi();

		// GET by id
		group.MapGet("/{id}", (int id) =>
		{
			using (var context = new EnterpriseContext())
			{
				return context.CipherSupports.Where(m => m.Id == id).ToList();
			}
		})
		.WithName("GetCipherSupportById")
		.WithOpenApi();

group.MapPut("/{id}", async (int id, CipherSupport input) =>
{
using var context = new EnterpriseContext();
var existing = context.CipherSupports.FirstOrDefault(m => m.Id == id);

if (existing == null)
{
return Results.NotFound();
}
existing.CipherName = input.CipherName;
existing.Version = input.Version;
existing.Description = input.Description;
existing.CipherKey = input.CipherKey;
existing.IsActive = input.IsActive;
existing.DateCreated = input.DateCreated;
await context.SaveChangesAsync();
return Results.Accepted($"/api/CipherSupport/{id}");
})
.WithName("UpdateCipherSupport")
.WithOpenApi();


		// POST create
		group.MapPost("/", async (CipherSupport input) =>
		{
			using (var context = new EnterpriseContext())
			{
				context.CipherSupports.Add(input);
				await context.SaveChangesAsync();
				return TypedResults.Created("Created ID:" + input.Id);
			}
		})
		.WithName("CreateCipherSupport")
		.WithOpenApi();

		// DELETE
		group.MapDelete("/{id}", async (int id) =>
		{
		using var context = new EnterpriseContext();
		var existing = context.CipherSupports.FirstOrDefault(m => m.Id == id);
		if (existing == null)
		{
			return Results.NotFound();
		}
		context.CipherSupports.Remove(existing);
		await context.SaveChangesAsync();
		return Results.Ok();
		})
		.WithName("DeleteCipherSupport")
		.WithOpenApi();

	}}

