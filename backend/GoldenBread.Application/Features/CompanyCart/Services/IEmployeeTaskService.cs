using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.CompanyCart.Services;

public interface IEmployeeTaskService
{
    Task CreateTasksForOrderAsync(
        Order order,
        List<CartItem> cartItems,
        CancellationToken cancellationToken);

    Task CancelTasksForOrderAsync(int orderId, CancellationToken cancellationToken);
}
