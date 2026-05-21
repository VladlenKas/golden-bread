using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Infrastructure.Services;

public class UnitConversionService : IUnitConversionService
{
    public decimal ToBaseUnit(decimal quantity, IngredientUnit unit) => unit switch
    {
        IngredientUnit.Kg => quantity * 1000,
        IngredientUnit.L => quantity * 1000,
        _ => quantity
    };

    public decimal FromBaseUnit(decimal baseQuantity, IngredientUnit targetUnit) => targetUnit switch
    {
        IngredientUnit.Kg => baseQuantity / 1000,
        IngredientUnit.L => baseQuantity / 1000,
        _ => baseQuantity
    };

    public bool CanConvert(IngredientUnit from, IngredientUnit to)
    {
        var massUnits = new[] { IngredientUnit.G, IngredientUnit.Kg };
        var volumeUnits = new[] { IngredientUnit.Ml, IngredientUnit.L };
        if (from == to) return true;
        if (massUnits.Contains(from) && massUnits.Contains(to)) return true;
        if (volumeUnits.Contains(from) && volumeUnits.Contains(to)) return true;
        return from == IngredientUnit.Pcs && to == IngredientUnit.Pcs;
    }
}