using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Infrastructure.Data.Repositories;

public class UserRepository(IGoldenBreadContext context) : IUserRepository
{
    public async Task<IReadOnlyList<Account>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Accounts
            .Where(a => a.AccountType == AccountType.User)
            .Include(a => a.User)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Users
            .Include(u => u.Account)
            .FirstOrDefaultAsync(u => u.UserId == id, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
    }
}

