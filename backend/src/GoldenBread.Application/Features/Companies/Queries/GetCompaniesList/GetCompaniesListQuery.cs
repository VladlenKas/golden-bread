using GoldenBread.Application.Features.Companies.Dtos;

namespace GoldenBread.Application.Features.Companies.Queries.GetCompaniesList;

public sealed record GetCompaniesListQuery : IRequest<CompaniesListResponse>;