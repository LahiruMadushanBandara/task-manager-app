using System.Text.Json;
using TaskManager.API.Helpers;
using TaskManager.API.Services.Contracts;

namespace TaskManager.API.Middleware;

public class BasicAuthMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> PublicRoutes =
        new(StringComparer.OrdinalIgnoreCase) { "/api/auth/verify" };

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        if (PublicRoutes.Contains(context.Request.Path.Value ?? string.Empty))
        {
            await next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.ToString();
        if (!BasicAuthParser.TryParse(authHeader, out var username, out var password))
        {
            await WriteUnauthorizedAsync(context, "Invalid or missing Authorization header.");
            return;
        }

        var userId = await authService.ValidateCredentialsAsync(username, password);
        if (userId is null)
        {
            await WriteUnauthorizedAsync(context, "Invalid username or password.");
            return;
        }

        context.Items["UserId"] = userId.Value;
        await next(context);
    }

    private static async Task WriteUnauthorizedAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new { success = false, message });
        await context.Response.WriteAsync(body);
    }
}
