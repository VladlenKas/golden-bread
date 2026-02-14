using GoldenBread.Application.Common.Behaviors.Authentication;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.Auth.Queries;

public sealed record class GetAccountBySessionQuery : AuthenticatedRequest<AuthResponse>;
