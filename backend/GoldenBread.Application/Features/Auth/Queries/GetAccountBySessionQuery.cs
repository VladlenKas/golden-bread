namespace GoldenBread.Application.Features.Auth.Queries;

public sealed record class GetAccountBySessionQuery(
    string Session) : IRequest<AuthResponse?>;
