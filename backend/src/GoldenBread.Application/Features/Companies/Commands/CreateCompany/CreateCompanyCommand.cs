using GoldenBread.Application.Features.Companies.Dtos;

namespace GoldenBread.Application.Features.Companies.Commands.CreateCompany;

public sealed record CreateCompanyCommand(CompanyDto CompanyDto, string Email, string Password) : IRequest<int>;