using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Domain.Services;

public class ProductionCalculator : IProductionCalculator
{
    private const int WorkDayMinutes = 8 * 60; // 480 минут

    public ProductionPlan CalculatePlan(
        IReadOnlyList<CartItem> cartItems,
        OrderTariff tariff,
        DateTime now,
        DateOnly? desiredDeliveryDate,
        IReadOnlyList<Employee> activeEmployees)
    {
        if (cartItems.Count == 0)
        {
            var tomorrow = DateOnly.FromDateTime(now.AddDays(1));
            return new ProductionPlan(
                tomorrow, tomorrow,
                now.AddDays(1), now.AddDays(1),
                0, Array.Empty<Employee>());
        }

        // 1. Определяем количество доступных сотрудников
        int availableEmployeesCount = Math.Max(
            1,
            (int)Math.Ceiling(activeEmployees.Count * tariff.FreeEmployeesPercent / 100m)
        );

        // 2. Рассчитываем общую трудоёмкость в минутах
        int totalProductionMinutes = cartItems.Sum(ci =>
            ci.Quantity * ci.Batch.Product.ProductionTimeMinutes * ci.Batch.QuantityUnits);

        // 3. Определяем период производства
        int totalWorkDaysNeeded = (int)Math.Ceiling((double)totalProductionMinutes / WorkDayMinutes);

        // 4. Выбираем наиболее свободных сотрудников
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

        // 5. Распределяем работу поровну
        int totalBatches = cartItems.Sum(ci => ci.Quantity);
        int minutesPerEmployee = (int)Math.Ceiling((double)totalProductionMinutes / availableEmployeesCount);
        int daysPerEmployee = (int)Math.Ceiling((double)minutesPerEmployee / WorkDayMinutes);

        // 6. Минимальная дата доставки
        DateOnly minimalDeliveryDate = DateOnly.FromDateTime(productionStart.AddDays(daysPerEmployee - 1));

        // 7. Итоговая дата доставки
        var deliveryDate = desiredDeliveryDate.HasValue && desiredDeliveryDate.Value >= minimalDeliveryDate
            ? desiredDeliveryDate.Value
            : minimalDeliveryDate;

        // 8. Пересчитываем productionEnd на основе итоговой даты
        var finalProductionEnd = productionStart.AddDays(daysPerEmployee);

        return new ProductionPlan(
            minimalDeliveryDate,
            deliveryDate,
            productionStart,
            finalProductionEnd,
            daysPerEmployee,
            selectedEmployees);
    }

    private int CalculateOverlapMinutes(EmployeeTask task, DateTime periodStart, DateTime periodEnd)
    {
        if (!task.StartTime.HasValue || !task.EndTime.HasValue) return 0;

        var overlapStart = task.StartTime.Value > periodStart ? task.StartTime.Value : periodStart;
        var overlapEnd = task.EndTime.Value < periodEnd ? task.EndTime.Value : periodEnd;

        if (overlapEnd <= overlapStart) return 0;
        return (int)(overlapEnd - overlapStart).TotalMinutes;
    }
}
