using GoldenBread.Application.Features.Users.Dtos;

namespace GoldenBread.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(UserDto UserDto) : IRequest<bool>;