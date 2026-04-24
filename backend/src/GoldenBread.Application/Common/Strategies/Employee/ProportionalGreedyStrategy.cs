using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Extensions;
using GoldenBread.Domain.Interfaces.Strategies;
using GoldenBread.Domain.ValueObjects;

namespace GoldenBread.Application.Common.Strategies.Employee;

public class ProportionalGreedyStrategy : IEmployeeSelectionStrategy
{
    public string Name => "Proportional by Free Time";

    public List<EmployeeAssignment> Select(
        OrderItem item,
        List<DbEntities.Employee> candidates,
        SchedulingContext context)
    {
        var totalUnits = item.Quantity * item.UnitsPerBatch;
        var minutesPerUnit = item.Batch.Product.ProductionTimeMinutes;
        var totalWorkMinutes = totalUnits * minutesPerUnit;

        // Берем 3 самых свободных сотрудника
        var topAll = candidates
            .OrderBy(e => context.CurrentLoadMinutes.GetValueOrDefault(e))
            .ToList();

        if (topAll.Count == 0) 
            return new List<EmployeeAssignment>();

        // Считаем: сколько у каждого осталось свободного времени до дедлайна?
        var now = DateTimeOffset.UtcNow;
        var capacities = topAll.Select(e => {
            var freeMinutes = e.GetFreeCapacityMinutes(now, context.Deadline, context.Calendar);
            return new
            {
                Employee = e,
                FreeCapacity = freeMinutes,
                CurrentLoad = context.CurrentLoadMinutes.GetValueOrDefault(e)
            };
        }).Where(c => c.FreeCapacity > 0) // Только те, кто хоть чем-то свободен
            .ToList();

        var totalFree = capacities.Sum(c => c.FreeCapacity);

        // Если всем вместе не хватит времени - заказ невозможен
        if (totalFree < totalWorkMinutes)
            return new List<EmployeeAssignment>();

        // Распределяем пропорционально свободному времени
        var assignments = new List<EmployeeAssignment>();
        var remaining = totalUnits;

        for (int i = 0; i < capacities.Count; i++)
        {
            var cap = capacities[i];

            // Доля = свободное время / общее свободное время
            var share = (int)Math.Round(totalUnits * (cap.FreeCapacity / totalFree));

            if (i == capacities.Count - 1) // Последний забирает остаток
                share = remaining;

            share = Math.Min(share, remaining);

            if (share > 0)
            {
                assignments.Add(new EmployeeAssignment(cap.Employee, share));

                // Обновляем загрузку в контексте
                context.CurrentLoadMinutes[cap.Employee] =
                    cap.CurrentLoad + (share * minutesPerUnit);

                remaining -= share;
            }
        }

        return assignments;
    }
}