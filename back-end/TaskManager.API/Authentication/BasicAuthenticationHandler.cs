using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TaskManager.API.Helpers;
using TaskManager.API.Services.Contracts;

namespace TaskManager.API.Authentication;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IAuthService authService)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Basic";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            return AuthenticateResult.NoResult();

        if (!BasicAuthParser.TryParse(authHeader.ToString(), out var username, out var password))
            return AuthenticateResult.Fail("Invalid Authorization header.");

        var userId = await authService.ValidateCredentialsAsync(username, password);
        if (userId is null)
            return AuthenticateResult.Fail("Invalid username or password.");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
            new Claim(ClaimTypes.Name, username),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    // Return JSON instead of a WWW-Authenticate header so the browser's native
    // Basic auth dialog never appears in front of the Angular login page.
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new { success = false, message = "Invalid or missing credentials." });
        await Response.WriteAsync(body);
    }
}
