using GoldenBread.Application.Features.Companies.Dtos;


namespace GoldenBread.Application.Features.Companies.Commands.UpdateCompany;

public sealed record UpdateCompanyCommand(CompanyDto CompanyDto) : IRequest<bool>;