using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;
using GoldenBread.Domain.Interfaces.Strategies;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Application.Common.Services;

public class ScheduleTaskDistributor(
    IWorkCalendar calendar,
    IEnumerable<IEmployeeSelectionStrategy> selectionStrategies,
    IEnumerable<IOrderItemOrderingStrategy> orderingStrategies)
{
    public ScheduleResult Distribute(
        List<OrderItem> orderItems,
        List<Employee> employees,
        ISchedulingStrategy schedulingStrategy,
        DateTimeOffset? deadline = null)
    {
        ScheduleResult? bestResult = null;

        foreach (var orderItemOrderingStrategy in orderingStrategies)
            foreach (var employeeTaskAssignmentStrategy in selectionStrategies)
            {
                // ⬇️ Клонируем сотрудников с задачами из БД, чтобы прогоны не мусорили друг друга
                var employeesSnapshot = employees.Select(e => Employee.Create(
                    e.EmployeeId,
                    e.Firstname,
                    e.Lastname,
                    e.Patronymic,
                    e.Birthday,
                    e.EmployeeTasks
                        .Where(t => t.StartTime.HasValue && t.EndTime.HasValue)
                        .Select(t => EmployeeTask.Create(
                            t.EmployeeId,
                            t.OrderItemId,
                            t.StartTime,
                            t.EndTime,
                            t.AssignedQuantity,
                            t.CompletedQuantity))
                        .ToList()
                )).ToList();

                var result = BuildSchedule(
                    orderItems,
                    employeesSnapshot,
                    employeeTaskAssignmentStrategy,
                    orderItemOrderingStrategy,
                    schedulingStrategy,
                    calendar,
                    deadline ?? DateTimeOffset.UtcNow.AddDays(calendar.MaxPlanningDays));

                if (bestResult == null || schedulingStrategy.IsBetter(result, bestResult))
                    bestResult = result;
            }

        return bestResult!;
    }

    private ScheduleResult BuildSchedule(
        List<OrderItem> orderItems,
        List<Employee> employees,
        IEmployeeSelectionStrategy selectionStrategy,
        IOrderItemOrderingStrategy orderingStrategy,
        ISchedulingStrategy schedulingStrategy,
        IWorkCalendar calendar,
        DateTimeOffset deadline)
    {
        var context = new SchedulingContext
        {
            Calendar = calendar,
            Deadline = deadline,
            CurrentLoadMinutes = employees.ToDictionary(e => e, e => 0.0)
        };

        var sortedItems = orderingStrategy.Sort(orderItems);
        var tasks = new List<EmployeeTask>();

        foreach (var item in sortedItems)
        {
            var assignments = selectionStrategy.Select(item, employees, context);

            if (assignments.Count == 0)
                return ScheduleResult.Failed($"Не удалось назначить позицию {item.OrderItemId}");

            foreach (var assignment in assignments)
            {
                var scheduledTasks = schedulingStrategy.TrySchedule(
                    assignment.Employee,
                    item,
                    assignment.AssignedQuantityUnits,
                    context.Deadline,
                    calendar);

                if (scheduledTasks.Count == 0)
                    return ScheduleResult.Failed("Не удалось разместить в календаре");

                foreach (var task in scheduledTasks)
                {
                    tasks.Add(task);
                    context.AssignedTasks.Add(task);
                    assignment.Employee.EmployeeTasks.Add(task);
                }
            }
        }

        if (tasks.Count == 0)
            return ScheduleResult.Failed("Нет назначенных задач");

        var planStart = tasks.Min(t => t.StartTime)!.Value.DateTime;
        var planEnd = tasks.Max(t => t.EndTime)!.Value.DateTime;

        return new ScheduleResult(
            Tasks: tasks,
            PlanStart: planStart,
            PlanEnd: planEnd,
            IsFeasible: true);
    }
}