namespace TaskManager.API.Middleware;

// Swallows cancellation from client-aborted requests (e.g. switchMap dropping a stale call).
public class RequestCancellationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = 499; // Client Closed Request
        }
    }
}
