using GoldenBread.Application.Features.Companies.Dtos;

namespace GoldenBread.Application.Features.Companies.Queries.GetCompanyById;

public sealed record GetCompanyByIdQuery(int Id) : IRequest<CompanyDto?>;