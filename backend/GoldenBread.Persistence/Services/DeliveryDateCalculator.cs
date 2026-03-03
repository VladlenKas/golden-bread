using GoldenBread.Application.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Infrastructure.Services;

public class DeliveryDateCalculator : IDeliveryDateCalculator
{
    private const int WorkDayMinutes = 8 * 60; // 8 часов = 480 минут

    public DateOnly CalculateMinimalDeliveryDate(
        List<CartItem> cartItems,
        OrderTariff tariff,
        List<Employee> activeEmployees,
        DateTime now)
    {
        if (cartItems.Count == 0)
            return DateOnly.FromDateTime(now.AddDays(1));

        // 1. Определяем количество доступных сотрудников по тарифу
        int availableEmployeesCount = Math.Max(
            1,
            (int)Math.Ceiling(activeEmployees.Count * tariff.MarkupPercent / 100m)
        );

        // 2. Рассчитываем общую трудоёмкость в минутах
        int totalProductionMinutes = cartItems.Sum(ci =>
            ci.Quantity * ci.Batch.Product.ProductionTimeMinutes * ci.Batch.QuantityPerBatch);

        // 3. Определяем период производства (предварительно)
        // Сначала считаем "в вакууме" сколько дней нужно
        int totalWorkDaysNeeded = (int)Math.Ceiling((double)totalProductionMinutes / WorkDayMinutes);

        // 4. Выбираем наиболее свободных сотрудников на этот период
        DateTime productionStart = now.Date.AddDays(1); // Завтра
        DateTime productionEnd = productionStart.AddDays(totalWorkDaysNeeded);

        var selectedEmployees = activeEmployees
            .Select(e => new
            {
                Employee = e,
                BusyMinutes = e.EmployeeTasks
                    .Where(et => et.EndTime > now && et.StartTime < productionEnd)
                    .Sum(et => CalculateOverlapMinutes(et, productionStart, productionEnd))
            })
            .OrderBy(x => x.BusyMinutes)
            .Take(availableEmployeesCount)
            .Select(x => x.Employee)
            .ToList();

        // 5. Распределяем работу поровну между выбранными сотрудниками
        int batchesCount = cartItems.Sum(ci => ci.Quantity);
        int batchesPerEmployee = (int)Math.Ceiling((double)batchesCount / availableEmployeesCount);

        // Минуты на одного сотрудника
        int minutesPerEmployee = batchesPerEmployee * cartItems
            .Select(ci => ci.Batch.Product.ProductionTimeMinutes * ci.Batch.QuantityPerBatch)
            .FirstOrDefault(); // Упрощение: берём среднее/первое, или пересчитываем точнее

        // Пересчитаем точнее: суммарные минуты / количество сотрудников
        minutesPerEmployee = (int)Math.Ceiling((double)totalProductionMinutes / availableEmployeesCount);

        // 6. Считаем рабочие дни для каждого сотрудника
        int daysPerEmployee = (int)Math.Ceiling((double)minutesPerEmployee / WorkDayMinutes);

        // 7. Минимальная дата = завтра + максимальные дни среди сотрудников
        // Так как распределение поровну, у всех одинаковые дни
        DateOnly minimalDate = DateOnly.FromDateTime(productionStart.AddDays(daysPerEmployee - 1));

        return minimalDate;
    }

    private int CalculateOverlapMinutes(EmployeeTask task, DateTime periodStart, DateTime periodEnd)
    {
        var overlapStart = task.StartTime > periodStart ? task.StartTime : periodStart;
        var overlapEnd = task.EndTime < periodEnd ? task.EndTime : periodEnd;

        if (overlapEnd <= overlapStart) return 0;

        return (int)(overlapEnd.Value - overlapStart.Value).TotalMinutes;
    }
}

