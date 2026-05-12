using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;
using System.Diagnostics;

namespace GoldenBread.Infrastructure.Data.Repositories;

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

    public async Task<Account?> GetBySessionAsync(
        string? session,
        CancellationToken ct = default)
    {
        return await context.Accounts
            .Include(a => a.Company)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a =>
                a.Session == session &&
                a.SessionExpiresAt > DateTime.UtcNow,
                ct);
    }

    public async Task<Account?> GetByIdAsync(
        int id, 
        CancellationToken ct = default)
    {
        return await context.Accounts
            .Include(a => a.Company)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AccountId == id, ct);
    }

    public async Task<Account?> GetByEmailAsync(
        string email,
        CancellationToken ct = default)
    {
        return await context.Accounts
            .Include(a => a.Company)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Email == email, ct);
    }

    public async Task<Account> AddAsync(
        Account account,
        CancellationToken ct = default)
    {
        await context.Accounts.AddAsync(account, ct);
        return account;
    }
}
