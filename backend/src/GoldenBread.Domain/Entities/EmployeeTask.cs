namespace GoldenBread.Domain.Entities;

public class EmployeeTask
{
    public int EmployeeTaskId { get; private set; }

    public int EmployeeId { get; private set; }
    public int OrderItemId { get; private set; }

    public int AssignedQuantity { get; private set; }
    public int CompletedQuantity { get; private set; }

    public DateTimeOffset? StartTime { get; private set; }
    public DateTimeOffset? EndTime { get; private set; }

    public Employee Employee { get; private set; } = null!;
    public OrderItem OrderItem { get; private set; } = null!;

    public Enums.TaskStatus Status { get; private set; }

    public EmployeeTask() { }

    public static EmployeeTask Create(
        int employeeId,
        int orderItemId,
        DateTimeOffset? startTime,
        DateTimeOffset? endTime,
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
            CompletedQuantity = completedQuantity,
        };
    }

    public void UpdateStatus(Enums.TaskStatus status)
    {
        if (status == Enums.TaskStatus.Completed)
            CompletedQuantity = AssignedQuantity;

        Status = status;
    }

    public void UpdateCompletedQuantity(int quantity)
    {
        if (quantity < 0 || quantity > AssignedQuantity)
            throw new ArgumentOutOfRangeException(nameof(quantity),
                "Количество выполненных единиц вне диапазона 0..AssignedQuantity");

        CompletedQuantity = quantity;
    }
}
