using GoldenBread.Desktop.Features.References.Products.Models;
using Refit;

namespace GoldenBread.Desktop.Infrastructure.Api;

public interface IIngredientsApi
{
    [Get("/api/ingredients")]
    Task<IApiResponse<IngredientsListResponse>> GetAll();

    [Get("/api/ingredients/{id}")]
    Task<IApiResponse<IngredientDto>> GetById(int id);

    [Post("/api/ingredients")]
    Task<IApiResponse<int>> Create([Body] IngredientDto dto);

    [Put("/api/ingredients")]
    Task<IApiResponse> Update([Body] IngredientDto dto);

    [Delete("/api/ingredients/{id}")]
    Task<IApiResponse> Delete(int id);
}
