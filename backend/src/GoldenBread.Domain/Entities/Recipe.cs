namespace GoldenBread.Domain.Entities;

public sealed class Recipe
{
    public int RecipeId { get; set; }

    public int ProductId { get; set; }
    public int IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public Ingredient Ingredient { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
