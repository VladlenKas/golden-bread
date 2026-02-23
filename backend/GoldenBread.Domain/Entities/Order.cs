using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Entities;

public class Order
{
    public int OrderId { get; set; }

    public int CompanyId { get; set; }
    public int TariffId { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public OrderTariff Tariff { get; set; } = null!;
    public Company Company { get; set; } = null!;
}
