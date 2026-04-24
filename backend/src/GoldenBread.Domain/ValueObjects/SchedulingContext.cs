using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Domain.ValueObjects;

public class SchedulingContext
{
    public Dictionary<Employee, double> CurrentLoadMinutes { get; set; } = new();
    public List<EmployeeTask> AssignedTasks { get; set; } = new();
    public IWorkCalendar Calendar { get; set; } = null!;
    public DateTimeOffset Deadline { get; set; }
}
