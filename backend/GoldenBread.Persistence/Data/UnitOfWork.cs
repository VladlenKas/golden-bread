using GoldenBread.Application.Common.Abstractions;

namespace GoldenBread.Infrastructure.Data;

internal class UnitOfWork(GoldenBreadContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
