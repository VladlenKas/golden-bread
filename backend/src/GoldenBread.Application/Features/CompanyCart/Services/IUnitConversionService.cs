using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.CompanyCart.Services;

public interface IUnitConversionService
{
    /// <summary>
    /// Конвертирует количество в базовую единицу (G, Ml, Pcs)
    /// </summary>
    decimal ToBaseUnit(decimal quantity, IngredientUnit unit);

    /// <summary>
    /// Конвертирует из базовой единицы в целевую
    /// </summary>
    decimal FromBaseUnit(decimal baseQuantity, IngredientUnit targetUnit);

    /// <summary>
    /// Проверяет, можно ли конвертировать между единицами (одна категория: масса/объём/шт)
    /// </summary>
    bool CanConvert(IngredientUnit from, IngredientUnit to);
}
