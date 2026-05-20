using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Domain.Interfaces.Strategies;

public interface ISchedulingStrategy
{
    /// <summary>
    /// Пытается разместить указанное количество единиц продукции в графике сотрудника.
    /// Возвращает список созданных задач. Если пустой — разместить не удалось.
    /// </summary>
    List<EmployeeTask> TrySchedule(
        Employee employee,
        OrderItem orderItem,
        int units,
        DateTimeOffset deadline,
        IWorkCalendar calendar);

    bool IsBetter(ScheduleResult current, ScheduleResult best);
}
