using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Models.DTOs;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    // The authentication handler validates credentials; reaching this action
    // means the request is already authenticated.
    [HttpPost("verify")]
    [Authorize]
    public IActionResult Verify()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok(ApiResponse<object>.Ok(new { userId }, "Authenticated successfully."));
    }
}
