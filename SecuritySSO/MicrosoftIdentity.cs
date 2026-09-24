using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Enterprise.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
[HttpGet("me")]
public IActionResult Me()
{
return Ok(new
{
Name = User.Identity?.Name,
ObjectId = User.FindFirst("oid")?.Value,
Email = User.FindFirst("preferred_username")?.Value
});
}
}
