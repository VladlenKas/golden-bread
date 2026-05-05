using GoldenBread.Desktop.Features.Administration.Users.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IUsersApi
{
    [Get("/api/users")]
    Task<IApiResponse<UsersListResponse>> GetAll();

    [Get("/api/users/{id}")]
    Task<IApiResponse<UserDto>> GetById(int id);

    [Post("/api/users")]
    Task<IApiResponse<int>> Create([Body] CreateUserRequest request);

    [Put("/api/users")]
    Task<IApiResponse> Update([Body] UserDto dto);
}
