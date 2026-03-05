namespace GoldenBread.Application.Abstractions.Services;

public interface ICookieService
{
    Task SignInAsync(string session);
    Task SignOutAsync();
}
