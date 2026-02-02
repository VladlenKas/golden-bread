using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Common.Abstractions.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task<Account?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<Account?> GetBySessionAsync(
        string session,
        CancellationToken cancellationToken);

    Task AddAsync(
        Account account,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Account account,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Account account,
        CancellationToken cancellationToken);
}
