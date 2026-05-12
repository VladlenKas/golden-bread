using GoldenBread.Application.Features.SupplierIngredient.Dtos;

namespace GoldenBread.Application.Features.SupplierIngredient.Queries.GetSupplierIngredientById;

public sealed record GetSupplierIngredientByIdQuery(int Id) : IRequest<SupplierIngredientDto?>;