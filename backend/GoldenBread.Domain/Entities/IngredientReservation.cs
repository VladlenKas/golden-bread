namespace GoldenBread.Domain.Entities;

public class IngredientReservation
{
    public int IngredientReservationId { get; private set; }

    public int OrderId { get; private set; }
    public int IngredientBatchId { get; private set; }

    public decimal ReservedQuantity { get; private set; }
    public DateTime ReservedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsConfirmed { get; private set; } = false;

    public Order Order { get; set; } = null!;
    public IngredientBatch Batch { get; set; } = null!;

    private IngredientReservation() { }

    public static IngredientReservation Create(
        int orderId,
        int ingredientBatchId,
        decimal quantity)
    {
        return new IngredientReservation
        {
            OrderId = orderId,
            IngredientBatchId = ingredientBatchId,
            ReservedQuantity = quantity,
            ReservedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    // Новый метод: окончательное списание при подтверждении отложенного заказа
    public void Confirm()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot confirm inactive reservation");

        if (IsConfirmed)
            throw new InvalidOperationException("Reservation already confirmed");

        IsConfirmed = true;
    }

    // Новый метод: отмена резерва (возврат ингредиентов)
    public void Cancel()
    {
        if (IsConfirmed)
            throw new InvalidOperationException("Cannot cancel confirmed reservation");

        IsActive = false;
    }
}
