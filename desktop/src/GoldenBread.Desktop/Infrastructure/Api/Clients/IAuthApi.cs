using GoldenBread.Desktop.Infrastructure.Api.Models.Auth;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api.Clients;

public interface IAuthApi
{
    [Post("/api/auth/login/user")]
    Task<IApiResponse<AuthResponse>> Login([Body] AuthRequest request);

    [Get("/api/auth/me")]
    Task<IApiResponse<AuthResponse?>> Me();

    [Post("/api/auth/logout")]
    Task<IApiResponse<object>> Logout();
}
