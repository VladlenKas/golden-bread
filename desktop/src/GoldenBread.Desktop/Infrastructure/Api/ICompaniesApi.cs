using GoldenBread.Desktop.Features.Administration.Companies.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface ICompaniesApi
{
    [Get("/api/companies")]
    Task<IApiResponse<CompaniesListResponse>> GetAll();

    [Get("/api/companies/{id}")]
    Task<IApiResponse<CompanyDto>> GetById(int id);

    [Post("/api/companies")]
    Task<IApiResponse<int>> Create([Body] CreateCompanyCommand command);

    [Put("/api/companies")]
    Task<IApiResponse> Update([Body] CompanyDto dto);
}
