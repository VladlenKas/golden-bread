using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Infrastructure.Data.Repositories;

internal class CompanyRepository(IGoldenBreadContext context) : ICompanyRepository
{
    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        name = name.Trim().ToLower();

        return await context.Companies.AnyAsync(c =>
            c.Name != null &&
            c.Name.Trim().ToLower() == name &&
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct);
    }

    public async Task<bool> ExistsByPhoneAsync(
        string? phone,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(phone)) 
            return false;

        return await context.Companies.AnyAsync(c =>
            c.Phone == phone &&
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct);
    }

    public async Task<bool> ExistsByAddressAsync(
        string? address,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(address))
            return false;

        address = address.Trim().ToLower();

        return await context.Companies.AnyAsync(c =>
            c.Address != null &&
            c.Address.Trim().ToLower() == address &&
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct);
    }

    public async Task<bool> ExistsByInnAsync(
        string inn,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        return await context.Companies.AnyAsync(c =>
            c.Inn == inn &&
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct);
    }

    public async Task<bool> ExistsByOgrnAsync(
        string ogrn,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        return await context.Companies.AnyAsync(c =>
            c.Ogrn == ogrn &&
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct);
    }

    public async Task<Company> AddAsync(
        Company company,
        CancellationToken ct = default)
    {
        await context.Companies.AddAsync(company, ct);
        return company;
    }

    public async Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Companies
            .Include(c => c.Account)
            .Include(c => c.Orders)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Company?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Companies
            .Include(c => c.Account)
            .FirstOrDefaultAsync(c => c.CompanyId == id, ct);
    }
}
