using GoldenBread.Desktop.Features.References.Products.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IProductCategoriesApi
{
    [Get("/api/product-categories")]
    Task<IApiResponse<ProductCategoriesListResponse>> GetAll();

    [Get("/api/product-categories/{id}")]
    Task<IApiResponse<ProductCategoryDto>> GetById(int id);

    [Post("/api/product-categories")]
    Task<IApiResponse<int>> Create([Body] ProductCategoryDto dto);

    [Put("/api/product-categories")]
    Task<IApiResponse> Update([Body] ProductCategoryDto dto);

    [Delete("/api/product-categories/{id}")]
    Task<IApiResponse> Delete(int id);
}