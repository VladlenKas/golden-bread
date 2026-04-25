namespace GoldenBread.Application.Abstractions.Services;

public interface ICookieService
{
    Task SignInWebAsync(string session);
    Task SignOutAsync();
}
