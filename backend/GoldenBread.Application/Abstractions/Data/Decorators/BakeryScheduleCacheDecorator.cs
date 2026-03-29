using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Application.Abstractions.Data.Decorators;

public class BakeryScheduleCacheDecorator(IBakeryScheduleService inner) : 
    IBakeryScheduleService
{
    private readonly Dictionary<DateTime, bool> _workDayCache = new();
    private readonly Dictionary<(DateTime, int), DateTime> _addWorkDaysCache = new();

    public bool IsWorkDay(DateTime date)
    {
        var key = date.Date;
        if (!_workDayCache.TryGetValue(key, out var result))
        {
            _workDayCache[key] = inner.IsWorkDay(date);
        }
        return result;
    }

    public DateTime AddWorkDays(DateTime start, int workDays)
    {
        var key = (start.Date, workDays);
        if (!_addWorkDaysCache.TryGetValue(key, out var result))
        {
            _addWorkDaysCache[key] = result = inner.AddWorkDays(start, workDays);
        }
        return result;
    }

    public DateTime AddWorkMinutes(DateTime start, int minutes) =>
        inner.AddWorkMinutes(start, minutes);

    public DateTime GetNextAvailableTime(Employee employee, DateTime from) =>
        inner.GetNextAvailableTime(employee, from);

    public DateTime GetWorkEnd(DateTime date) =>
        inner.GetWorkEnd(date); 

    public DateTime GetWorkStart(DateTime date) =>
        inner.GetWorkStart(date); 

    public DateTime SnapToWorkTime(DateTime date) =>
        inner.SnapToWorkTime(date);
}
