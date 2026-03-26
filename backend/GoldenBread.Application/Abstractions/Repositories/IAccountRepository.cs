using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Abstractions.Repositories;

public interface IAccountRepository
{
    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken ct = default);
    Task<Account?> GetBySessionAsync(string? session, CancellationToken ct);
    Task<Account?> GetByEmailAsync(string email, CancellationToken ct);
    Task<Account> AddAsync(Account account, CancellationToken ct);
}
