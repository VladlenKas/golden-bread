using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Common.Exceptions.Domain;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Data.Repositories;

internal class CompanyRepository(IGoldenBreadContext context) : ICompanyRepository
{
    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        return await context.Companies.AnyAsync(c =>
            c.Name == name &&
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct);
    }

    public async Task<bool> ExistsByPhoneAsync(
        string? phone,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;

        return await context.Companies.AnyAsync(c =>
            c.Phone == phone &&
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct);
    }

    public async Task<bool> ExistsByAddressAsync(
        string? address,
        int? excludeId = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(address)) return false;

        return await context.Companies.AnyAsync(c =>
            c.Address == address &&
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

    public async Task<Company> AddAsync(Company company, CancellationToken ct)
    {
        await context.Companies.AddAsync(company, ct);
        return company;
    }
}
