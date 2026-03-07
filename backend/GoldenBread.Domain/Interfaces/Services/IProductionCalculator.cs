using GoldenBread.Domain.Entities;
namespace GoldenBread.Domain.Interfaces.Services;

public interface IProductionCalculator
{
    ProductionPlan CalculatePlan(
        IReadOnlyList<CartItem> cartItems,
        OrderTariff tariff,
        DateTime now,
        DateOnly? desiredDeliveryDate,
        IReadOnlyList<Employee> activeEmployees);
}

public record ProductionPlan(
    DateOnly MinimalDeliveryDate,
    DateOnly ConfirmedDeliveryDate,
    DateTime ProductionStart,
    DateTime ProductionEnd,
    int RequiredWorkDays,
    IReadOnlyList<Employee> SelectedEmployees);


