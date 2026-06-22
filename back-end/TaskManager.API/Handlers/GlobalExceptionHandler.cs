using Microsoft.AspNetCore.Diagnostics;
using TaskManager.API.Models.DTOs;

namespace TaskManager.API.Handlers;

// Logs unhandled exceptions and returns a 500 in the ApiResponse envelope.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception for {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail("An unexpected error occurred. Please try again later."),
            cancellationToken);

        return true;
    }
}
