using GoldenBread.Domain.Enums;
using System;

namespace GoldenBread.Domain.Entities;

public class IngredientBatch
{
    public int IngredientBatchId { get; set; }

    public int IngredientId { get; set; }

    public int PurchasedQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public DateOnly DeliveryDate { get; set; }
    public DateOnly ExpiryDate { get; set; }

    public IngredientBatchStatus Status { get; set; }

    public Ingredient Ingredient { get; set; } = null!;
}
