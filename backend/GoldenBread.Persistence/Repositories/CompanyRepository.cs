using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Domain.Entities;
using GoldenBread.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Repositories;

internal sealed class CompanyRepository(GoldenBreadContext context) : ICompanyRepository
{
    public async Task AddAsync(Company company, CancellationToken cancellationToken)
    {
        await context.Companies.AddAsync(company, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Company company, CancellationToken cancellationToken)
    {
        context.Companies.Update(company);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Company company, CancellationToken cancellationToken)
    {
        context.Companies.Remove(company);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Company?>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Companies
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Company?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CompanyId == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Company, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await context.Companies
            .AnyAsync(predicate, cancellationToken);
    }
}
