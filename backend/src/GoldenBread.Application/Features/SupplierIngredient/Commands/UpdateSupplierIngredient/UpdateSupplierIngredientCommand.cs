using GoldenBread.Application.Features.SupplierIngredient.Dtos;

namespace GoldenBread.Application.Features.SupplierIngredient.Commands.UpdateSupplierIngredient;

public sealed record UpdateSupplierIngredientCommand(SupplierIngredientDto Dto) : IRequest<bool>;
