namespace GoldenBread.Application.Common.Abstractions.Services;

public interface ICookieService
{
    Task SignInAsync(string session);
    string? GetSession();
    Task SignOutAsync();
}
