using GoldenBread.Application.Common.Abstractions.Data;

namespace GoldenBread.Application.Common.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(IGoldenBreadContext context)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return response;
    }
}
