namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed record class LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse?>;
