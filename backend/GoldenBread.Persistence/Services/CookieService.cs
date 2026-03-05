using GoldenBread.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GoldenBread.Infrastructure.Services;

public sealed class CookieService(
    IHttpContextAccessor httpContextAccessor) : ICookieService
{
    public async Task SignInAsync(string session)
    {
        var claims = new[]
        {
            new Claim("session", session)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await httpContextAccessor.HttpContext!
            .SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
    }

    public async Task SignOutAsync()
    {
        await httpContextAccessor.HttpContext!
            .SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
