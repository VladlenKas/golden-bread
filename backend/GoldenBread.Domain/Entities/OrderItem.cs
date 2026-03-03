namespace GoldenBread.Domain.Entities;

public class OrderItem
{
    public int OrderItemId { get; private set; }

    public int OrderId { get; private set; }
    public int? BatchId { get; private set; }

    public int Quantity { get; set; }
    public int? TotalUnits { get; set; }
    public decimal? UnitPriceAtOrder { get; set; }

    public ProductBatch Batch { get; set; } = null!;
    public Order Order { get; set; } = null!;

    public ICollection<EmployeeTask> EmployeeTasks { get; set; } = new List<EmployeeTask>();

    public OrderItem() { }
}
