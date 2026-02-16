using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Behaviors.Authentication;

public abstract record class AuthenticatedRequest<TResponse>() : IAuthenticatedRequest<TResponse>
{
    public Account Account { get; set; } = null!;
}
