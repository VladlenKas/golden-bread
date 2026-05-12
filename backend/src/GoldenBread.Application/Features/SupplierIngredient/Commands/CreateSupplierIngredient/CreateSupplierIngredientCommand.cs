using GoldenBread.Application.Features.SupplierIngredient.Dtos;

namespace GoldenBread.Application.Features.SupplierIngredient.Commands.CreateSupplierIngredient;

public sealed record CreateSupplierIngredientCommand(SupplierIngredientDto SupplierIngredientDto) : IRequest<int>;