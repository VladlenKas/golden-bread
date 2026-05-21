using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Abstractions.Services;

public interface IUnitConversionService
{
    decimal ToBaseUnit(decimal quantity, IngredientUnit unit);
    decimal FromBaseUnit(decimal baseQuantity, IngredientUnit targetUnit);
    bool CanConvert(IngredientUnit from, IngredientUnit to);
}