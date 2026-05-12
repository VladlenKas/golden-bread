using GoldenBread.Desktop.Features.Procurement.PurchasePositions.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface ISupplierIngredientsApi
{
    [Get("/api/supplier-ingredients")]
    Task<IApiResponse<SupplierIngredientsListResponse>> GetList();

    [Get("/api/supplier-ingredients/{id}")]
    Task<IApiResponse<SupplierIngredientDto>> GetById(int id);

    [Post("/api/supplier-ingredients")]
    Task<IApiResponse<int>> Create([Body] SupplierIngredientDto dto);

    [Put("/api/supplier-ingredients/{id}")]
    Task<IApiResponse> Update(int id, [Body] SupplierIngredientDto dto);

    [Delete("/api/supplier-ingredients/{id}")]
    Task<IApiResponse> Delete(int id);
}