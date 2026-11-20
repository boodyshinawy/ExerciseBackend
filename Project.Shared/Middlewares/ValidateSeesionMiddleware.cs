using Microsoft.AspNetCore.Http;

namespace Project.Shared;

public class ValidateSessionMiddleware
{
    private readonly RequestDelegate _next;

    public ValidateSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var sessionIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "sessionId");
            if (sessionIdClaim != null)
            {
                var sessionId = sessionIdClaim.Value;
                var session = await dbContext.Set<UserSession>()
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.IsActive);

                if (session == null)
                {
                    // If the session is not active, do not process the request further
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Session is not active.");
                    return;
                }
            }
        }

        await _next(context);
    }
}