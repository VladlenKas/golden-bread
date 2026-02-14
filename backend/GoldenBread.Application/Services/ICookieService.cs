namespace GoldenBread.Application.Services;

public interface ICookieService
{
    Task SignInAsync(string session);
    Task SignOutAsync();
}
