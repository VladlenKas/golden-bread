namespace GoldenBread.Domain.Entities;

public class EmployeeTask
{
    public int EmployeeTaskId { get; private set; }

    public int EmployeeId { get; private set; }
    public int OrderItemId { get; private set; }

    public int AssignedQuantity { get; set; }
    public int CompletedQuantity { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public Employee Employee { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;

    public EmployeeTask() { }

    public static EmployeeTask Create(
        int employeeId,
        int orderItemId,
        DateTime? startTime,
        DateTime? endTime,
        int assignedQuantity,
        int completedQuantity)
    {
        return new EmployeeTask
        {
            EmployeeId = employeeId,
            OrderItemId = orderItemId,
            StartTime = startTime,
            EndTime = endTime,
            AssignedQuantity = assignedQuantity,
            CompletedQuantity = completedQuantity
        };
    }
}
