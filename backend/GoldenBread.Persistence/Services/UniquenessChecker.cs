using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Exceptions;
using GoldenBread.Application.Services;

namespace GoldenBread.Infrastructure.Services;

public class UniquenessChecker(IGoldenBreadContext context) : IUniquenessChecker
{
    public async Task CompanyNameMustBeUniqueAsync(
        string name, 
        int? excludeId = null, 
        CancellationToken ct = default)
    {
        if (await context.Companies.AnyAsync(c => 
            c.Name == name && 
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct))
            throw new NameDuplicateException();
    }

    public async Task CompanyPhoneMustBeUniqueAsync(
        string? phone, 
        int? excludeId = null, 
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(phone)) return;
        if (await context.Companies.AnyAsync(c => 
            c.Phone == phone && 
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct))
            throw new PhoneDuplicateException();
    }

    public async Task CompanyAddressMustBeUniqueAsync(
        string? address, 
        int? excludeId = null, 
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(address)) return;
        if (await context.Companies.AnyAsync(c => 
            c.Address == address && 
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct))
            throw new AddressDuplicateException();
    }

    public async Task CompanyInnMustBeUniqueAsync(
        string inn, 
        int? excludeId = null, 
        CancellationToken ct = default)
    {
        if (await context.Companies.AnyAsync(c => 
            c.Inn == inn && 
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct))
            throw new InnDuplicateException();
    }

    public async Task CompanyOgrnMustBeUniqueAsync(
        string ogrn, 
        int? excludeId = null, 
        CancellationToken ct = default)
    {
        if (await context.Companies.AnyAsync(c => 
            c.Ogrn == ogrn && 
            (!excludeId.HasValue || c.CompanyId != excludeId.Value), ct))
            throw new OgrnDuplicateException();
    }

    public async Task EmailMustBeUniqueAsync(
        string email, 
        int? excludeId = null, 
        CancellationToken ct = default)
    {
        if (await context.Accounts.AnyAsync(a => 
            a.Email == email && 
            (!excludeId.HasValue || a.AccountId != excludeId.Value), ct))
            throw new EmailDuplicateException();
    }
}
