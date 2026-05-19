using GoldenBread.Desktop.Features.References.Products.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IProductsApi
{
    [Get("/api/products")]
    Task<IApiResponse<ProductsListResponse>> GetAll();

    [Get("/api/products/{id}")]
    Task<IApiResponse<ProductDto>> GetById(int id);

    [Get("/api/products/{id}/detail")]
    Task<IApiResponse<ProductDetailResponse>> GetDetail(int id);

    [Post("/api/products")]
    Task<IApiResponse<int>> Create([Body] CreateProductWithDetailsCommand command);

    [Put("/api/products")]
    Task<IApiResponse> Update([Body] ProductDto dto);

    [Put("/api/products/recipe")]
    Task<IApiResponse> UpdateRecipe([Body] UpdateProductRecipeCommand command);

    [Put("/api/products/batches")]
    Task<IApiResponse> UpdateBatches([Body] UpdateProductBatchesCommand command);

    [Put("/api/products/images")]
    Task<IApiResponse> UpdateImages([Body] UpdateProductImagesCommand command);

    [Delete("/api/products/{id}")]
    Task<IApiResponse> Delete(int id);
}
