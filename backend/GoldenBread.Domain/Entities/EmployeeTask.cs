using System;

namespace GoldenBread.Domain.Entities;

public class EmployeeTask
{
    public int EmployeeTaskId { get; set; }

    public int EmployeeId { get; set; }
    public int OrderItemId { get; set; }

    public int AssignedQuantity { get; set; }
    public int CompletedQuantity { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public Employee Employee { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
}
