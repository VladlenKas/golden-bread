using GoldenBread.Application.Features.Auth.Dtos;

namespace GoldenBread.Application.Features.Auth.Queries.GetAccountBySession;

public sealed record class GetAccountBySessionQuery : 
    IRequest<AuthResponse?>;
