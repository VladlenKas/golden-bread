using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.Companies.Dtos;
using GoldenBread.Domain.Entities;
using System.Security.Principal;


namespace GoldenBread.Application.Features.Companies.Queries.GetCompaniesList;

public sealed class GetCompaniesListQueryHandler(ICompanyRepository companyRepository)
    : IRequestHandler<GetCompaniesListQuery, CompaniesListResponse>
{
    public async Task<CompaniesListResponse> Handle(GetCompaniesListQuery query, CancellationToken ct)
    {
        var accountsCompanies = await companyRepository.GetAllAsync(ct);

        var list = accountsCompanies
            .Select(c => new CompanyListItem(
                c.Account.AccountId,
                c.CompanyId,
                c.Name,
                c.Inn,
                c.Ogrn,
                c.Phone,
                c.Address,
                c.Account.Email,
                c.Account.VerificationStatus,
                c.Account.CreatedAt,
                c.Account.SessionExpiresAt,
                CanDelete(c)))
            .ToList();

        return new CompaniesListResponse(list);
    }

    private bool CanDelete(Company company)
    {
        return !company.Orders.Any(o =>
                o.Status == Domain.Enums.OrderStatus.InProgress ||
                o.Status == Domain.Enums.OrderStatus.Awaiting) ||
                company.Orders.Count == 0;
    }
}
