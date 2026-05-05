using GoldenBread.Application.Features.Users.Dtos;

namespace GoldenBread.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(int Id) : IRequest<UserDto?>;