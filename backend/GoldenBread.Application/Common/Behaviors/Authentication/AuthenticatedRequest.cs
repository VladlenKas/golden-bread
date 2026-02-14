using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Common.Behaviors.Authentication;

public abstract record class AuthenticatedRequest<TResponse> :
    IRequest<TResponse>, IAuthenticatedRequest<TResponse> 
{
    public Account Account { get; set; } = null!;
}
