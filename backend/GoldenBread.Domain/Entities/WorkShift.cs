namespace GoldenBread.Domain.Entities;

public class WorkShift
{
    public int ShiftId { get; private set; }

    public int EmployeeId { get; private set; }
    public DateOnly ShiftDate { get; set; }

    public TimeOnly StartTime { get; set; } = new TimeOnly(8, 0); // '08:00'
    public TimeOnly BreakStart { get; set; } = new TimeOnly(12, 0); // '12:00'
    public TimeOnly BreakEnd { get; set; } = new TimeOnly(13, 0); // '13:00'
    public TimeOnly EndTime { get; set; } = new TimeOnly(17, 0); // '17:00'

    public bool IsWorking { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Employee Employee { get; set; } = null!;
    public ICollection<OrderProductionPlan> OrderProductionPlans { get; set; } = new List<OrderProductionPlan>();

    public WorkShift() { }

    public static WorkShift Create(
        int employeeId,
        DateOnly shiftDate,
        TimeOnly? startTime,
        TimeOnly? breakStart,
        TimeOnly? breakEnd,
        TimeOnly? endTime,
        bool isWorking)
    {
        return new WorkShift
        {
            EmployeeId = employeeId,
            ShiftDate = shiftDate,
            StartTime = startTime ?? new TimeOnly(8, 0),
            BreakStart = breakStart ?? new TimeOnly(12, 0),
            BreakEnd = breakEnd ?? new TimeOnly(13, 0),
            EndTime = endTime ?? new TimeOnly(17, 0),
            IsWorking = isWorking,
            CreatedAt = DateTime.UtcNow
        };
    }
}
