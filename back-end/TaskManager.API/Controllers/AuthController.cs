using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Helpers;
using TaskManager.API.Models.DTOs;
using TaskManager.API.Services.Contracts;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("verify")]
    public async Task<IActionResult> Verify()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (!BasicAuthParser.TryParse(authHeader, out var username, out var password))
            return Unauthorized(ApiResponse<object>.Fail("Invalid or missing Authorization header."));

        var userId = await authService.ValidateCredentialsAsync(username, password);
        if (userId is null)
            return Unauthorized(ApiResponse<object>.Fail("Invalid username or password."));

        return Ok(ApiResponse<object>.Ok(new { userId }, "Authenticated successfully."));
    }
}
