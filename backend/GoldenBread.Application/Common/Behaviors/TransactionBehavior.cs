using GoldenBread.Application.Common.Abstractions;

namespace GoldenBread.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await next();
    }
}
