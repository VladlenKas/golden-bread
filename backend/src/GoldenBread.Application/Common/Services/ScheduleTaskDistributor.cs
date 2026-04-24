using GoldenBread.Application.Common.Strategies.Employee;
using GoldenBread.Application.Common.Strategies.Product;
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
        ISchedulingStrategy schedulingStrategy)
    {
        ScheduleResult? bestResult = null;

        foreach (var orderItemOrderingStrategy in orderingStrategies)
            foreach (var employeeTaskAssignmentStrategy in selectionStrategies)
            {
                var result = BuildSchedule(
                    orderItems,
                    employees,
                    employeeTaskAssignmentStrategy,
                    orderItemOrderingStrategy,
                    schedulingStrategy,
                    calendar);

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
        IWorkCalendar calendar)
    {
        // Инициализируем контекст
        var context = new SchedulingContext
        {
            Calendar = calendar,
            Deadline = DateTimeOffset.UtcNow.AddDays(calendar.MaxPlanningDays),
            CurrentLoadMinutes = employees.ToDictionary(
                e => e,
                e => 0.0) // Начальная загрузка - 0
        };

        var sortedItems = orderingStrategy.Sort(orderItems);
        var tasks = new List<EmployeeTask>();

        foreach (var item in sortedItems)
        {
            // 1. Выбираем сотрудника (с учётом текущей загрузки в контексте!)
            var assignments = selectionStrategy.Select(item, employees, context);

            if (assignments.Count == 0)
                return ScheduleResult.Failed($"Не удалось назначить позицию {item.OrderItemId}");

            // 2. Планируем время
            foreach (var assignment in assignments)
            {
                var task = schedulingStrategy.TrySchedule(
                    assignment.Employee,
                    item,
                    assignment.AssignedQuantityUnits,
                    context.Deadline,
                    calendar);

                if (task == null)
                    return ScheduleResult.Failed("Не удалось разместить в календаре");

                tasks.Add(task);
                context.AssignedTasks.Add(task); // Обновляем контекст задачами тоже
            }
        }

        // Считаем общие метрики плана
        var planStart = tasks.Min(t => t.StartTime)?.DateTime ?? DateTime.MinValue;
        var planEnd = tasks.Max(t => t.EndTime)?.DateTime ?? DateTime.MaxValue;

        return new ScheduleResult(
            Tasks: tasks,
            PlanStart: planStart,
            PlanEnd: planEnd,
            IsFeasible: true);
    }
}