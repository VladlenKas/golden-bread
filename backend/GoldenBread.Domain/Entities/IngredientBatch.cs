using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public class IngredientBatch
{
    public int IngredientBatchId { get; private set; }

    public int IngredientId { get; private set; }

    public int PurchasedQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public DateOnly DeliveryDate { get; set; }
    public DateOnly ExpiryDate { get; set; }

    public IngredientBatchStatus Status { get; set; }

    public Ingredient Ingredient { get; set; } = null!;

    public IngredientBatch() { }

    public static IngredientBatch Create(
        int ingredientBatchId,
        int ingredientId,
        int purchasedQuantity,
        decimal remainingQuantity,
        DateOnly deliveryDate,
        DateOnly expiryDate,
        IngredientBatchStatus status)
    {
        return new IngredientBatch
        {
            IngredientBatchId = ingredientBatchId,
            IngredientId = ingredientId,
            PurchasedQuantity = purchasedQuantity,
            RemainingQuantity = remainingQuantity,
            DeliveryDate = deliveryDate,
            ExpiryDate = expiryDate,
            Status = status
        };
    }

    public static IngredientBatch Create(
        int ingredientBatchId,
        Ingredient ingredient,
        int purchasedQuantity,
        decimal remainingQuantity,
        DateOnly deliveryDate,
        DateOnly expiryDate,
        IngredientBatchStatus status)
    {
        var item = Create(
            ingredientBatchId,
            ingredient.IngredientId,
            purchasedQuantity,
            remainingQuantity,
            deliveryDate,
            expiryDate,
            status);

        item.Ingredient = ingredient;
        return item;
    }
}
