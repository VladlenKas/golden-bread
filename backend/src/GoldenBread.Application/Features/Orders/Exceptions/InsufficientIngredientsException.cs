using GoldenBread.Application.Features.Orders.Dtos;

namespace GoldenBread.Application.Features.Orders.Exceptions;

public class InsufficientIngredientsException : Exception
{
    public List<IngredientShortageItem> Shortages { get; }

    public InsufficientIngredientsException(List<IngredientShortageItem> shortages)
        : base("Недостаточно ингредиентов для запуска заказа в производство")
    {
        Shortages = shortages;
    }
}