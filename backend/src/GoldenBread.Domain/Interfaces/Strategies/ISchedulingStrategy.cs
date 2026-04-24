using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Domain.Interfaces.Strategies;

public interface ISchedulingStrategy
{
    // Возвращаем готовое назначение или null, если не получилось
    EmployeeTask? TrySchedule(
        Employee employee,
        OrderItem orderItem,
        int units,
        DateTimeOffset deadline,
        IWorkCalendar calendar);

    bool IsBetter(ScheduleResult current, ScheduleResult best);
}
