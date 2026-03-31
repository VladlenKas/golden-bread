using GoldenBread.Domain.Constants;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Domain.Services;

public class EmployeeTaskDistributor(IWorkScheduleService scheduleCalculator) : IEmployeeTaskDistributor
{
    public IReadOnlyList<EmployeeTaskAssignment> Distribute(
        OrderItem orderItem,
        IReadOnlyList<Employee> employees,
        Dictionary<int, DateTime> employeeAvailableFrom,
        decimal freeEmployeesPercent,
        DateTime currentTime)
    {
        if (employees.Count == 0 ||
            orderItem.Quantity <= 0 ||
            orderItem.Batch?.Product == null)
            return Array.Empty<EmployeeTaskAssignment>();

        // Точное количество единиц продукции (историческое значение!)
        int totalUnits = orderItem.Quantity;
        int unitsPerBatch = orderItem.UnitsPerBatch; // НЕ Batch.QuantityUnits!
        int minutesPerUnit = orderItem.Batch.Product.ProductionTimeMinutes;

        if (unitsPerBatch <= 0 || minutesPerUnit <= 0)
            return Array.Empty<EmployeeTaskAssignment>();

        // Количество сотрудников для распределения
        int employeesCount = Math.Min(
            employees.Count,
            Math.Max(1, (int)Math.Ceiling(employees.Count * Math.Clamp(freeEmployeesPercent, 1, 100) / 100m))
        );

        // Не берём больше сотрудников, чем единиц продукции (1 штука = минимум)
        employeesCount = Math.Min(employeesCount, totalUnits);

        var selectedEmployees = employees
            .Select(e => new
            {
                Employee = e,
                AvailableFrom = employeeAvailableFrom.TryGetValue(e.EmployeeId, out var from)
                    ? from
                    : scheduleCalculator.GetNextAvailableTime(e, currentTime)
            })
            .OrderBy(x => x.AvailableFrom)
            .ThenBy(x => x.Employee.EmployeeId)
            .Take(employeesCount)
            .Select(x => x.Employee)
            .ToList();

        // Распределяем ЕДИНИЦЫ поровну
        int baseUnitsPerEmployee = totalUnits / employeesCount;
        int extraUnits = totalUnits % employeesCount;

        var assignments = new List<EmployeeTaskAssignment>();
        int assignedUnits = 0;

        for (int i = 0; i < selectedEmployees.Count; i++)
        {
            var employee = selectedEmployees[i];

            // Последние "extraUnits" сотрудников получают +1 единица
            int unitsForThisEmployee = baseUnitsPerEmployee + (i < extraUnits ? 1 : 0);

            if (unitsForThisEmployee == 0) continue;

            // Время = количество единиц × время на единицу
            int totalMinutes = unitsForThisEmployee * minutesPerUnit;

            // Когда сотрудник свободен
            var availableFrom = employeeAvailableFrom.TryGetValue(employee.EmployeeId, out var from)
                ? from
                : scheduleCalculator.GetNextAvailableTime(employee, currentTime);

            // Прилипаем к рабочему времени
            var taskStart = scheduleCalculator.SnapToWorkTime(availableFrom);
            var taskEnd = scheduleCalculator.AddWorkMinutes(taskStart, totalMinutes);

            var deadline = orderItem.Order?.EndDate;
            if (deadline.HasValue)
            {
                // Дедлайн — конец рабочего дня даты доставки
                var deadlineDateTime = deadline.Value.ToDateTime(
                    new TimeOnly(WorkScheduleConstants.WorkEndHour, 0));

                if (taskEnd > deadlineDateTime)
                {
                    throw new InvalidOperationException(
                        $"Order deadline exceeded: Employee {employee.EmployeeId} would finish at {taskEnd}, " +
                        $"but order must be completed by {deadlineDateTime}. " +
                        $"Consider upgrading tariff or changing delivery date.");
                }
            }

            var nextAvailable = scheduleCalculator.SnapToWorkTime(taskEnd);

            assignments.Add(new EmployeeTaskAssignment(
                employee.EmployeeId,
                orderItem.OrderItemId,
                taskStart,
                taskEnd,
                unitsForThisEmployee)); // Точное количество единиц!

            assignedUnits += unitsForThisEmployee;
            employeeAvailableFrom[employee.EmployeeId] = nextAvailable;
        }

        // Защита: проверяем, что распределили ровно столько, сколько заказали
        if (assignedUnits != totalUnits)
            throw new InvalidOperationException($"Distribution error: assigned {assignedUnits} of {totalUnits}");

        return assignments;
    }
}
