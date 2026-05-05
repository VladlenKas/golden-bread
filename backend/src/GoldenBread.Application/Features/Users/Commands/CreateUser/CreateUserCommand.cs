using GoldenBread.Application.Features.Users.Dtos;

namespace GoldenBread.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(UserDto UserDto, string Email, string Password) : IRequest<int>;
