using GoldenBread.Desktop.Features.Common.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IAuthApi
{
    [Post("/api/auth/login/user")]
    Task<IApiResponse<AuthResponse>> Login([Body] AuthRequest request);

    [Get("/api/auth/me")]
    Task<IApiResponse<AuthResponse?>> Me();

    [Post("/api/auth/logout")]
    Task<IApiResponse<object>> Logout();
}
