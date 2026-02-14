using GoldenBread.Application.Common.Behaviors.Authentication;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Auth.Commands.Login;

public sealed record class LogoutCommand() : AuthenticatedRequest<Unit>;
