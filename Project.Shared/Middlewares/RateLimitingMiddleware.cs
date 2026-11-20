using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Project.Shared;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static MemoryCache Cache = new(new MemoryCacheOptions());

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var path = context.Request.Path.ToString();
        var key = $"{ipAddress}-{path}";

        if (!Cache.TryGetValue(key, out int requestCount))
        {
            requestCount = 0;
        }

        if (requestCount >= 100) // For example, limit to 100 requests per minute
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            return;
        }
        else
        {
            Cache.Set(key, requestCount + 1, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
        }

        await _next(context);
    }
}


