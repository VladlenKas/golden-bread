using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public class Order
{
    public int OrderId { get; set; }

    public int CompanyId { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CanceledAt { get; private set; }
    public string? CancelReason { get; private set; }

    public OrderStatus Status { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public Company Company { get; set; } = null!;

    public static Order Create(
        int companyId,
        OrderStatus status,
        DateOnly desiredDeliveryDate)
    {
        return new Order
        {
            CompanyId = companyId,
            Status = status,
            EndDate = desiredDeliveryDate,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
    }

    public void Cancel(string? reason = null)
    {
        Status = OrderStatus.Canceled;
        CanceledAt = DateTime.UtcNow;
        CancelReason = reason;
    }
}
