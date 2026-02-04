using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Domain.Entities;
using GoldenBread.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Infrastructure.Repositories;

internal sealed class AccountRepository(
    GoldenBreadContext context) : IAccountRepository
{
    public async Task AddAsync(
        Account account,
        CancellationToken cancellationToken)
    {
        await context.Accounts.AddAsync(account, cancellationToken);
    }

    public async Task DeleteAsync(
        Account account,
        CancellationToken cancellationToken)
    {
        account.IsActive = 0;
    }

    public async Task<Account?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Email == email,
                cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.AccountId == id,
                cancellationToken);
    }

    public async Task<Account?> GetBySessionAsync(
        string session,
        CancellationToken cancellationToken)
    {
        return await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Session == session,
                cancellationToken);
    }

    public async Task UpdateAsync(
        Account account,
        CancellationToken cancellationToken)
    {
        context.Accounts.Update(account);
    }
}
