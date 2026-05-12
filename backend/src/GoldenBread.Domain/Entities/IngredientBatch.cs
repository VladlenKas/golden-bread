using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public class IngredientBatch
{
    public int IngredientBatchId { get; private set; }

    public int SupplierIngredientId { get; private set; }

    public int PurchasedQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public DateOnly DeliveryDate { get; set; }
    public DateOnly ExpiryDate { get; set; }

    public IngredientBatchStatus Status { get; set; }

    public SupplierIngredient SupplierIngredient { get; set; } = null!;

    public IngredientBatch() { }

    public static IngredientBatch Create(
        int supplierIngredientId,
        int purchasedQuantity,
        decimal remainingQuantity,
        DateOnly deliveryDate,
        DateOnly expiryDate,
        IngredientBatchStatus status)
    {
        return new IngredientBatch
        {
            SupplierIngredientId = supplierIngredientId,
            PurchasedQuantity = purchasedQuantity,
            RemainingQuantity = remainingQuantity,
            DeliveryDate = deliveryDate,
            ExpiryDate = expiryDate,
            Status = status
        };
    }

    public void SetArchivedStatus()
    {
        Status = IngredientBatchStatus.Archived;
    }

    public decimal TryWriteOff(decimal amount)
    {
        var toWriteOff = Math.Min(RemainingQuantity, amount);
        RemainingQuantity -= toWriteOff;

        if (RemainingQuantity == 0)
            Status = IngredientBatchStatus.OutOfStock;

        return toWriteOff; // Возвращаем сколько реально списали
    }
}
