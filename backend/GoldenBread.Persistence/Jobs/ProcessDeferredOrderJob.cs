using GoldenBread.Application.Abstractions.Repositories;
using GoldenBread.Application.Features.CompanyOrder.Services;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Infrastructure.Jobs;

public class ProcessDeferredOrderJob(
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    IIngredientReservationService ingredientService,
    IEmployeeTaskRepository taskRepository)
{
    public async Task Execute(int orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null || order.Status != OrderStatus.AwaitingIngredients)
            return;

        // Проверяем ингредиенты снова
        var orderItems = await orderItemRepository.GetByOrderIdAsync(orderId);
        var check = await ingredientService.CheckAsync(orderItems);

        if (check.IsSufficient)
        {
            // Подтверждаем резервы и меняем статус
            await ingredientService.ConfirmReservationsAsync(orderId);
            order.UpdateStatus(OrderStatus.Awaiting);
            await orderRepository.UpdateAsync(order);
        }
        else
        {
            // Отменяем заказ
            order.Cancel("Insufficient ingredients after 3 days");
            await orderRepository.UpdateAsync(order);

            // Удаляем задачи сотрудников
            await taskRepository.DeleteByOrderIdAsync(orderId);

            // Отменяем резервы
            await ingredientService.CancelReservationsAsync(orderId);
        }
    }
}

