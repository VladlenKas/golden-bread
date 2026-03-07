using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Domain.Services;

public class EmployeeTaskDistributor(IWorkScheduleCalculator scheduleCalculator) : IEmployeeTaskDistributor
{
    public IReadOnlyList<EmployeeTaskAssignment> Distribute(
        OrderItem orderItem,
        IReadOnlyList<Employee> employees,
        Dictionary<int, DateTime> employeeAvailableFrom,
        decimal freeEmployeesPercent)
    {
        if (employees.Count == 0 || orderItem.QuantityPerBatch == 0)
            return Array.Empty<EmployeeTaskAssignment>();

        var assignments = new List<EmployeeTaskAssignment>();

        // Количество сотрудников для распределения
        int employeesCount = Math.Max(1,
            (int)Math.Ceiling(employees.Count * freeEmployeesPercent / 100m));

        // Берём самых свободных (уже отсортированы по загрузке вне этого метода)
        var selectedEmployees = employees.Take(employeesCount).ToList();

        // Распределяем партии поровну
        int totalBatches = orderItem.QuantityPerBatch;
        int baseBatchesPerEmployee = totalBatches / employeesCount;
        int extraBatches = totalBatches % employeesCount;

        int assignedBatches = 0;

        for (int i = 0; i < selectedEmployees.Count && assignedBatches < totalBatches; i++)
        {
            var employee = selectedEmployees[i];
            int batchesForThisEmployee = baseBatchesPerEmployee + (i < extraBatches ? 1 : 0);

            if (batchesForThisEmployee == 0) continue;

            // Время на эту задачу
            int minutesPerBatch = orderItem.Batch.Product.ProductionTimeMinutes * orderItem.Batch.QuantityPerBatch;
            int totalMinutes = batchesForThisEmployee * minutesPerBatch;

            // Определяем когда сотрудник свободен
            var availableFrom = employeeAvailableFrom.GetValueOrDefault(
                employee.EmployeeId,
                scheduleCalculator.GetWorkStart(DateTime.UtcNow.AddDays(1)));

            // Рассчитываем StartTime и EndTime с учётом рабочего дня и перерыва
            var taskStart = availableFrom;
            var taskEnd = scheduleCalculator.AddWorkMinutes(taskStart, totalMinutes);

            assignments.Add(new EmployeeTaskAssignment(
                employee.EmployeeId,
                orderItem.OrderItemId,
                taskStart,
                taskEnd,
                batchesForThisEmployee));

            assignedBatches += batchesForThisEmployee;

            // Обновляем доступность сотрудника (параллельно не работаем, задачи последовательны для одного сотрудника)
            employeeAvailableFrom[employee.EmployeeId] = taskEnd;
        }

        return assignments;
    }
}

