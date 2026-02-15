using GoldenBread.Application.Behaviors.Authentication;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed record class LogoutCommand() : AuthenticatedRequest<Unit>;
