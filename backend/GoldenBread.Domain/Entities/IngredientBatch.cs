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
        int ingredientId,
        int purchasedQuantity,
        decimal remainingQuantity,
        DateOnly deliveryDate,
        DateOnly expiryDate,
        IngredientBatchStatus status)
    {
        return new IngredientBatch
        {
            IngredientId = ingredientId,
            PurchasedQuantity = purchasedQuantity,
            RemainingQuantity = remainingQuantity,
            DeliveryDate = deliveryDate,
            ExpiryDate = expiryDate,
            Status = status
        };
    }
}
