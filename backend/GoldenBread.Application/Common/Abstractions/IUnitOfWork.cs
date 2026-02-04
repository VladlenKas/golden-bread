namespace GoldenBread.Application.Common.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
