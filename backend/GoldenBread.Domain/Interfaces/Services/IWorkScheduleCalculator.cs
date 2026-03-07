namespace GoldenBread.Domain.Interfaces.Services;

public interface IWorkScheduleCalculator
{
    DateTime GetWorkStart(DateTime date);
    DateTime GetWorkEnd(DateTime date);
    bool IsWorkDay(DateTime date);
    DateTime AddWorkDays(DateTime start, int workDays);
    DateTime AddWorkMinutes(DateTime start, int minutes);
    int CalculateWorkMinutes(DateTime start, DateTime end);
}
