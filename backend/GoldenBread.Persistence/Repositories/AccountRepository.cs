using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;
using GoldenBread.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Repositories;

internal class AccountRepository(GoldenBreadContext context) : IAccountRepository
{
    public async Task AddAsync(Account account)
    {
        await context.Accounts.AddAsync(account);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Account account)
    {
        account.IsActive = 1;
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Account?>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Account?> GetByEmailAsync(string email)
    {
        return await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Account?> GetByIdAsync(int id)
    {
        return await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.AccountId == id);
    }

    public async Task UpdateAsync(Account account)
    {
        context.Accounts.Update(account);
        await context.SaveChangesAsync();
    }
}