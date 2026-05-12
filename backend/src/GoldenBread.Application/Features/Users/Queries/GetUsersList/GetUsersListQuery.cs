using GoldenBread.Application.Features.Users.Dtos;

namespace GoldenBread.Application.Features.Users.Queries.GetUsersList;

public sealed record GetUsersListQuery : IRequest<UsersListResponse>;