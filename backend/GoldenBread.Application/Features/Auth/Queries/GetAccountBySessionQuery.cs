using GoldenBread.Application.Behaviors.Authentication;

namespace GoldenBread.Application.Features.Auth.Queries;

public sealed record class GetAccountBySessionQuery() : AuthenticatedRequest<AuthResponse>;
