using GoldenBread.Desktop.Features.References.Suppliers.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface ISuppliersApi
{
    [Get("/api/suppliers")]
    Task<IApiResponse<SuppliersListResponse>> GetAll();

    [Get("/api/suppliers/{id}")]
    Task<IApiResponse<SupplierDto>> GetById(int id);

    [Post("/api/suppliers")]
    Task<IApiResponse<int>> Create([Body] SupplierDto dto);

    [Put("/api/suppliers")]
    Task<IApiResponse> Update([Body] SupplierDto dto);

    [Delete("/api/suppliers/{id}")]
    Task<IApiResponse> Delete(int id);
}