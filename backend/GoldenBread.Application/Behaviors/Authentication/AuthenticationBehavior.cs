using GoldenBread.Application.Services;

namespace GoldenBread.Application.Behaviors.Authentication;

public sealed class AuthenticationBehavior<TRequest, TResponse>(ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IAuthenticatedRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var account = await currentUserService.Account(cancellationToken);
        request.Account = account ?? throw new UnauthorizedAccessException();
        return await next(cancellationToken);
    }
}
