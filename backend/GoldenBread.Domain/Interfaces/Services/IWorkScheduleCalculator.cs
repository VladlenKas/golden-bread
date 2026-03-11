using GoldenBread.Domain.Entities;

namespace GoldenBread.Domain.Interfaces.Services;

public interface IWorkScheduleCalculator
{
    DateTime GetWorkStart(DateTime date);
    DateTime GetWorkEnd(DateTime date);
    bool IsWorkDay(DateTime date);
    DateTime AddWorkMinutes(DateTime start, int minutes);
    DateTime AddWorkDays(DateTime start, int workDays);
    DateTime GetNextAvailableTime(Employee employee, DateTime from);
    DateTime SnapToWorkTime(DateTime date);
}
