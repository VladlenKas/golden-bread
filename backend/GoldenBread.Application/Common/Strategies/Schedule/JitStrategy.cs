using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.Interfaces.Strategies;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Application.Common.Strategies.Schedule;

public class JitStrategy : ISchedulingStrategy
{
    public bool IsBetter(
        ScheduleResult current, 
        ScheduleResult best)
    {
        throw new NotImplementedException();
    }

    public EmployeeTask? TrySchedule(
        DbEntities.Employee employee, 
        OrderItem orderItem, 
        int units,
        DateTimeOffset deadline, 
        IWorkCalendar calendar)
    {
        throw new NotImplementedException();
    }
}
