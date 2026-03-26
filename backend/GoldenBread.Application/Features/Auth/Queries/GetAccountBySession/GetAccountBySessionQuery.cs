using GoldenBread.Application.Features.Auth.Dtos;

namespace GoldenBread.Application.Features.Auth.Queries.GetAccountBySession;

public sealed record GetAccountBySessionQuery : IRequest<AuthResponse?>;
