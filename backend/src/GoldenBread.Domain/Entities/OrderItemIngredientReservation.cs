namespace GoldenBread.Domain.Entities;

public class OrderItemIngredientReservation
{
    public int OrderItemIngredientReservationId { get; set; }
    public int OrderItemId { get; set; }
    public int IngredientBatchId { get; set; }
    public decimal ReservedQuantity { get; set; }   // В единице поставщика

    public OrderItem OrderItem { get; set; } = null!;
    public IngredientBatch IngredientBatch { get; set; } = null!;
}