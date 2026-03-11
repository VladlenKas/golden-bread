using GoldenBread.Application.Features.CompanyCart.Services;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Infrastructure.Services;

public class DeliveryDateCalculator(IWorkScheduleCalculator workSchedule) : IDeliveryDateCalculator
{
    private const int MaxPlanningDays = 60;

    public DateOnly CalculateMinimalDeliveryDate(
        List<CartItem> cartItems,
        OrderTariff tariff,
        List<Employee> activeEmployees,
        DateTime now)
    {
        if (!cartItems.Any())
            return DateOnly.FromDateTime(workSchedule.AddWorkDays(now, 1));

        // 1. Сколько сотрудников можем использовать
        int maxEmployees = Math.Max(1,
            (int)Math.Ceiling(activeEmployees.Count * tariff.FreeEmployeesPercent / 100m));

        // 2. Общая работа
        var batches = CreateBatchList(cartItems);
        var totalWorkMinutes = batches.Sum(b => b.DurationMinutes);

        // 3. Подготавливаем таймлайны сотрудников
        var productionStart = workSchedule.AddWorkDays(now, 1); // Первый рабочий день

        var employeeTimelines = activeEmployees
            .Select(e => CreateTimeline(e, productionStart))
            .OrderBy(t => t.NextAvailable)
            .Take(maxEmployees)
            .ToList();

        if (!employeeTimelines.Any())
            throw new InvalidOperationException("No available employees");

        // 4. Распределяем партии с учётом рабочего графика
        foreach (var batch in batches)
        {
            var employee = employeeTimelines
                .OrderBy(e => e.GetNextAvailableSlot(batch.DurationMinutes, workSchedule))
                .First();

            var slotStart = employee.GetNextAvailableSlot(batch.DurationMinutes, workSchedule);
            var slotEnd = workSchedule.AddWorkMinutes(slotStart, batch.DurationMinutes);

            employee.AddTask(slotStart, slotEnd, workSchedule);
        }

        // 5. Когда освободится последний?
        var completionTime = employeeTimelines.Max(e => e.NextAvailable);

        // Доставка — в конец рабочего дня завершения (или следующий)
        var deliveryDate = DateOnly.FromDateTime(completionTime);

        // Если закончили после 15:00 — доставка на следующий рабочий день
        var completionHour = completionTime.Hour;
        if (completionHour >= 15 || !workSchedule.IsWorkDay(completionTime))
            deliveryDate = DateOnly.FromDateTime(workSchedule.AddWorkDays(completionTime, 1));

        return deliveryDate;
    }

    private EmployeeTimeline CreateTimeline(
        Employee employee, 
        DateTime from)
    {
        // Загружаем задачи на период планирования
        var existingTasks = employee.EmployeeTasks
            .Where(t => t.EndTime.HasValue && t.EndTime.Value > from && t.StartTime < from.AddDays(MaxPlanningDays))
            .Select(t => new WorkSlot(t.StartTime!.Value, t.EndTime!.Value))
            .ToList();

        // Когда сотрудник освободится по рабочему графику
        var nextAvailable = workSchedule.GetNextAvailableTime(employee, from);

        return new EmployeeTimeline(employee.EmployeeId, nextAvailable, existingTasks);
    }

    private List<BatchWork> CreateBatchList(List<CartItem> cartItems)
    {
        var batches = new List<BatchWork>();

        foreach (var item in cartItems)
        {
            var duration = item.Batch.Product.ProductionTimeMinutes * item.Batch.QuantityPerBatch;

            for (int i = 0; i < item.Quantity; i++)
            {
                batches.Add(new BatchWork(item.CartItemId, i + 1, duration, item.Batch.Product.Name));
            }
        }

        return batches.OrderByDescending(b => b.DurationMinutes).ToList();
    }
}

public class EmployeeTimeline
{
    public int EmployeeId { get; }
    public DateTime NextAvailable { get; private set; }
    private readonly List<WorkSlot> _existingTasks;
    private readonly List<WorkSlot> _newTasks = new();

    public EmployeeTimeline(int id, DateTime available, List<WorkSlot> existing)
    {
        EmployeeId = id;
        NextAvailable = available;
        _existingTasks = existing.OrderBy(t => t.Start).ToList();
    }

    /// <summary>
    /// Находит ближайший слот для задачи указанной длительности
    /// </summary>
    public DateTime GetNextAvailableSlot(int durationMinutes, IWorkScheduleCalculator workSchedule)
    {
        var pointer = NextAvailable;
        var maxIterations = 100; // Защита от бесконечного цикла

        for (int i = 0; i < maxIterations; i++)
        {
            // Проверяем, не пересекается ли с существующими задачами
            var conflict = _existingTasks
                .Concat(_newTasks)
                .Where(t => t.Start < workSchedule.AddWorkMinutes(pointer, durationMinutes) && t.End > pointer)
                .OrderBy(t => t.Start)
                .FirstOrDefault();

            if (conflict == null)
                return pointer; // Нашли свободный слот

            // Сдвигаемся за конфликт и "прилипаем" к рабочему времени
            pointer = workSchedule.SnapToWorkTime(conflict.End);
        }

        throw new InvalidOperationException("Cannot find available slot within planning horizon");
    }

    public void AddTask(DateTime start, DateTime end, IWorkScheduleCalculator workSchedule)
    {
        _newTasks.Add(new WorkSlot(start, end));
        NextAvailable = workSchedule.SnapToWorkTime(end);
    }
}

public record WorkSlot(
    DateTime Start, 
    DateTime End);

public record BatchWork(
    int CartItemId, 
    int BatchNumber, 
    int DurationMinutes, 
    string ProductName);
