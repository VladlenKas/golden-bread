namespace GoldenBread.Application.Common.Abstractions.Services;

public interface ICookieService
{
    Task SignInAsync(string session);
    Task<string?> FindMeAsync();
    Task SignOutAsync();
}
