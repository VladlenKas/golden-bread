using GoldenBread.Application.Abstractions.Data.Repositories;


namespace GoldenBread.Application.Features.Companies.Queries.GetCompaniesList;

public sealed class GetCompaniesListQueryHandler(ICompanyRepository companyRepository)
    : IRequestHandler<GetCompaniesListQuery, CompaniesListResponse>
{
    public async Task<CompaniesListResponse> Handle(GetCompaniesListQuery query, CancellationToken ct)
    {
        var accountsCompanies = await companyRepository.GetAllAsync(ct);

        var list = accountsCompanies
            .Select(a => new CompanyListItem(
                a.AccountId,
                a.Company!.CompanyId,
                a.Company!.Name,
                a.Company!.Inn,
                a.Company!.Ogrn,
                a.Company!.Phone,
                a.Company!.Address,
                a.Email,
                a.VerificationStatus,
                a.CreatedAt,
                a.SessionExpiresAt))
            .ToList();

        return new CompaniesListResponse(list);
    }
}