namespace GoldenBread.Application.Common.Exceptions.Domain;

public class InsufficientIngredientsException : BusinessValidationException
{
    public int IngredientId { get; }

    public InsufficientIngredientsException(int ingredientId)
        : base("IngredientId", $"Недостаточно ингредиентов (ID: {ingredientId})")
    {
        IngredientId = ingredientId;
    }
}