using System;
using System.Collections.Generic;

namespace GoldenBread.Domain.Entities;

public class OrderTariff
{
    public int OrderTariffId { get; set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal MarkupPercent { get; set; }
    public decimal FreeEmployeesPercent { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
