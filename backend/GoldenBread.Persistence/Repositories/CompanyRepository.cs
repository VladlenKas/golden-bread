using GoldenBread.Application.Common.Abstractions.Repositories;
using GoldenBread.Domain.Entities;
using GoldenBread.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Repositories;

internal class CompanyRepository(GoldenBreadContext context) : ICompanyRepository
{
    public async Task AddAsync(Company company)
    {
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company company)
    {
        context.Companies.Update(company);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Company company)
    {
        context.Companies.Remove(company);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Company?>> GetAllAsync()
    {
        return await context.Companies
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Company?> GetByIdAsync(int id)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CompanyId == id);
    }

    public async Task<Company?> GetByInnAsync(string inn)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Inn == inn);
    }

    public async Task<Company?> GetByNameAsync(string name)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<Company?> GetByOgrnAsync(string ogrn)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Ogrn == ogrn);
    }

    public async Task<Company?> GetByPhoneAsync(string phone)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Phone == phone);
    }

    public async Task<Company?> GetByAddressAsync(string address)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Address == address);
    }
}
