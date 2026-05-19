using GoldenBread.Desktop.Features.Common;

namespace GoldenBread.Desktop.UI.Helpers;

public static class LocalizedIngredientUnits
{
    public sealed record UnitFilterOption(IngredientUnit? Value, string Display);

    /// <summary>
    /// Для форм
    /// </summary>
    public static Dictionary<IngredientUnit, string> Units { get; } = new()
    {
        [IngredientUnit.G] = "Грамм",
        [IngredientUnit.Kg] = "Килограмм",
        [IngredientUnit.Ml] = "Миллилитр",
        [IngredientUnit.L] = "Литр",
        [IngredientUnit.Pcs] = "Штук"
    };

    /// <summary>
    ///  Для фильтров
    /// </summary>
    public static List<UnitFilterOption> UnitsFilters { get; } = new()
    {
        new(null, "Все ед. измер."),
        new(IngredientUnit.G, "В граммах"),
        new(IngredientUnit.Kg, "В килограммах"),
        new(IngredientUnit.Ml, "В миллилитрах"),
        new(IngredientUnit.L, " В литрах"),
        new(IngredientUnit.Pcs, "В штуках"),
    };

    /// <summary>
    /// Для таблиц и списков
    /// </summary>
    public static string UnitsTable(IngredientUnit unit) => unit switch
    {
        IngredientUnit.G => "г.",
        IngredientUnit.Kg => "кг.",
        IngredientUnit.Ml => "мл.",
        IngredientUnit.L => "л.",
        IngredientUnit.Pcs => "шт.",
        _ => "-"
    };

    /// <summary>
    /// Для детального просмотра
    /// </summary>
    public static string UnitsDetail(string unit) => unit switch
    {
        "G" => "г.",
        "Kg" => "кг.",
        "Ml" => "мл.",
        "L" => "л.",
        "Pcs" => "шт.",
        _ => "-"
    };
}
