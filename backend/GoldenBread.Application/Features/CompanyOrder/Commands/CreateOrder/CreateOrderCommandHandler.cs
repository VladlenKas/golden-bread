using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.CompanyOrder.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.CompanyOrder.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    ICurrentAccountContext accountContext,
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    ICartRepository cartRepository,
    IEmployeeRepository employeeRepository,
    IEmployeeTaskRepository employeeTaskRepository) :
    IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(
        CreateOrderCommand command,
        CancellationToken ct)
    {
        // 1. Получить корзину
        int companyId = await accountContext.GetRequiredCompanyIdAsync(ct);
        var cartItems = await cartRepository.GetByCompanyIdAsync(companyId, ct);

        // 2. Создать заказ
        var order = Order.Create(
            companyId,
            command.TariffId,
            OrderStatus.Awaiting,
            command.DesiredDeliveryDate);

        await orderRepository.CreateAsync(order, ct);

        // 3. Создать OrderItems
        var orderItems = cartItems.Select(ci => OrderItem.Create(
            order.OrderId, ci.BatchId, ci.Quantity, ci.Batch.QuantityUnits, ci.Batch.UnitPrice))
            .ToList();

        await orderItemRepository.CreateRangeAsync(orderItems, ct);

        // 4. Очистить корзину
        await cartRepository.ClearAsync(companyId, ct);

        return new CreateOrderResult
        {
            Success = true,
            OrderId = order.OrderId,
            DeliveryDate = command.DesiredDeliveryDate,
            IsDeferred = false
        };
    }
}
