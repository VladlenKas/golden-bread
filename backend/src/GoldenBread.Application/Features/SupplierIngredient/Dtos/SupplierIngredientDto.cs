using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.SupplierIngredient.Dtos;

public record class SupplierIngredientDto(
    int SupplierIngredientId,
    int SupplierId,
    int IngredientId,
    string Name,
    decimal Price,
    IngredientUnit Unit,
    decimal Weight,
    int ShelfLifeDays);
