using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.CompanyCart.Services;

public interface IDeliveryDateCalculator
{
    DateOnly CalculateMinimalDeliveryDate(
        List<CartItem> cartItems,
        OrderTariff tariff,
        List<Employee> activeEmployees,
        DateTime now);

}
