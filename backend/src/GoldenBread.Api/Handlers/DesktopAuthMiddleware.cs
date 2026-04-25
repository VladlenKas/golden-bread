using System.Security.Claims;

namespace GoldenBread.Api.Handlers;

public class DesktopAuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            var session = context.Request.Headers["X-Session-Desktop"]
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(session))
            {
                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        [new Claim("session", session)],
                        "DesktopScheme"));
            }
        }

        await next(context);
    }
}