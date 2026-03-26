namespace GoldenBread.Domain.Entities;

public class OrderProductionPlan
{
    public int PlanId { get; private set; }

    public int OrderId { get; private set; }
    public int OrderItemId { get; private set; }

    public int EmployeeId { get; private set; }
    public int PlannedShiftId { get; private set; }

    public int PlannedMinutes { get; private set; }

    public int AssignedBatches { get; private set; } = 1;
    public int CompletedBatches { get; private set; } = 0;

    public bool IsCompleted => CompletedBatches >= AssignedBatches;

    public Order Order { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public WorkShift Shift { get; set; } = null!;

    public OrderProductionPlan() { }

    public static OrderProductionPlan Create(
        int orderId,
        int orderItemId,
        int employeeId,
        int plannedShiftId,
        int plannedMinutes,
        int assignedBatches)
    {
        return new OrderProductionPlan
        {
            OrderId = orderId,
            OrderItemId = orderItemId,
            EmployeeId = employeeId,
            PlannedShiftId = plannedShiftId,
            PlannedMinutes = plannedMinutes,
            AssignedBatches = assignedBatches,
            CompletedBatches = 0
        };
    }

    public void CompleteBatch()
    {
        if (CompletedBatches < AssignedBatches)
            CompletedBatches++;
    }
}
