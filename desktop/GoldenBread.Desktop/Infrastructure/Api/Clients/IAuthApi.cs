using GoldenBread.Desktop.Infrastructure.Api.Models.Auth;
using Refit;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Infrastructure.Api.Clients;

public interface IAuthApi
{
    [Post("/auth/login/user")]
    Task<IApiResponse<AuthResponse>> Login([Body] AuthRequest request);

    [Get("/api/auth/me")]
    [Headers("X-Desktop-Session: ")] 
    Task<IApiResponse<AuthResponse?>> Me();

    [Post("/api/auth/logout")]
    Task<IApiResponse<object>> Logout();
}
