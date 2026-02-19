namespace GoldenBread.Domain.Entities;

public sealed class OrderTariff
{
    public int OrderTariffId { get; set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal MarkupPercent { get; set; }
    public decimal FreeEmployeesPercent { get; set; }
    public short IsDelete { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
