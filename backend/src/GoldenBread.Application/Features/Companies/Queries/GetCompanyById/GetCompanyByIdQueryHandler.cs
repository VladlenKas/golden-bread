using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.Companies.Dtos;


namespace GoldenBread.Application.Features.Companies.Queries.GetCompanyById;

public sealed class GetCompanyByIdQueryHandler(ICompanyRepository companyRepository)
    : IRequestHandler<GetCompanyByIdQuery, CompanyDto?>
{
    public async Task<CompanyDto?> Handle(GetCompanyByIdQuery query, CancellationToken ct)
    {
        var company = await companyRepository.GetByIdAsync(query.Id, ct);
        if (company is null)
            return null;

        return new CompanyDto(
            company.CompanyId,
            company.Name,
            company.Inn,
            company.Ogrn,
            company.Phone,
            company.Address);
    }
}