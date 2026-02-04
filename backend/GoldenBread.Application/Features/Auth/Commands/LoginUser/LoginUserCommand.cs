using GoldenBread.Contracts.Responses;

namespace GoldenBread.Application.Features.Auth.Commands.LoginUser;

public sealed record class LoginUserCommand(
    string Email,
    string Password) : IRequest<LoginUserResponse?>;
