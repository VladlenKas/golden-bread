using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

#warning Добавить default к ct (во всех репах)
internal class AccountRepository
    (IGoldenBreadContext context) :
    IAccountRepository
{
    public async Task<bool> ExistsByEmailAsync(
        string email,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        return await context.Accounts.AnyAsync(a =>
            a.Email == email &&
            (!excludeId.HasValue || a.AccountId != excludeId.Value), ct);
    }

    public async Task<Account?> GetBySessionAsync(string? session, CancellationToken ct)
    {
        return await context.Accounts
            .FirstOrDefaultAsync(a =>
                a.Session == session &&
                a.SessionExpiresAt > DateTime.UtcNow,
                ct);
    }

    public async Task<Account?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await context.Accounts
            .FirstOrDefaultAsync(a => a.Email == email, ct);
    }

    public async Task<Account> AddAsync(Account account, CancellationToken ct)
    {
        await context.Accounts.AddAsync(account, ct);
        return account;
    }
}
