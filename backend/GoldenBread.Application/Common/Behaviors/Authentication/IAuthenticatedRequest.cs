using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Common.Behaviors.Authentication;

public interface IAuthenticatedRequest<TResponse> : IRequest<TResponse>
{
    Account Account { get; set; } 
}
    