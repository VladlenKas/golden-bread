using GoldenBread.Domain.Entities;

namespace GoldenBread.Domain.ValueObjects;

public record ScheduleResult(
    List<EmployeeTask>? Tasks,
    DateTime PlanStart,
    DateTime PlanEnd,
    bool IsFeasible,
    string? CancelReason = null)
{
    public static ScheduleResult Failed(string error) =>
        new(new List<EmployeeTask>(), DateTime.MinValue, DateTime.MinValue, false, error);
}