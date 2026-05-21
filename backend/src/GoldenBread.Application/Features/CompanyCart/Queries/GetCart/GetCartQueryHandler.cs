using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Common.Services;
using GoldenBread.Application.Common.Strategies.Schedule;
using GoldenBread.Application.Features.CompanyCart.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Interfaces.Services;

namespace GoldenBread.Application.Features.CompanyCart.Queries.GetCart;

#warning Перенести проекцию в сервис Infra
public class GetCartQueryHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext,
    ScheduleTaskDistributor scheduler) :
    IRequestHandler<GetCartQuery, CartDto>
{
    public async Task<CartDto> Handle(
        GetCartQuery request,
        CancellationToken ct)
    {
        int companyId = await accountContext.GetRequiredCompanyIdAsync(ct);

        var items = await context.CartItems.Where(ci => ci.CompanyId == companyId).ToListAsync(cancellationToken: ct);
        if (items.Count <= 0)
        {
            return new CartDto
            {
                CartItemsList = null,
                MinimalDeliveryDate = null,
                MaximalDeliveryDate = null
            };
        }

        // Загружаем позиции корзины для DTO
        var productCartItemsDto = await context.CartItems
            .AsNoTracking()
            .Include(ci => ci.Batch)
                .ThenInclude(b => b.Product)
            .Where(ci => ci.CompanyId == companyId)
            .Select(ci => new ProductCartItemDto
            {
                ProductId = ci.Batch.ProductId,
                Name = ci.Batch.Product.Name,
                ProductionTimeMinutes = ci.Batch.Product.ProductionTimeMinutes,
                ProductBatchId = ci.Batch.ProductBatchId,
                QuantityPerBatch = ci.Batch.QuantityUnits,
                ImageUrl = ci.Batch.Product.ProductImages
                    .Select(pi => pi.ImagePath)
                    .FirstOrDefault(),
                IsFavorite = ci.Batch.Product.Favorites
                    .Any(f => f.CompanyId == companyId),
                QuantityInCart = ci.Quantity,
                TotalCostInCart = ci.TotalCost
            })
            .ToListAsync(ct);

        // Грузим корзину для определенныя даты доставки
        var cartItems = await context.CartItems
            .AsNoTracking()
            .Include(ci => ci.Batch)
                .ThenInclude(b => b.Product)
            .Where(ci => 
                ci.CompanyId == companyId)
            .ToListAsync(ct);

        // Грузим сотрудников
        var employees = await context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeTasks)
            .ToListAsync(ct);

        // Создаем временный лист позиций заказа
        List<OrderItem> orderItems = new();

        // Преобразуем позиции корзниы в позиции заказа
        foreach(var cartItem in cartItems)
        {
            var orderItem = OrderItem.Create(
                orderId: 0, // временный
                batchId: cartItem.BatchId,
                quantity: cartItem.Quantity,
                unitsPerBatch: cartItem.Batch.QuantityUnits,
                unitPrice: cartItem.Batch.UnitPrice);

            orderItem.Batch = cartItem.Batch;

            orderItems.Add(orderItem);
        }

        // Задаем ASAP стратегию и объявляем планировщик
        var asap = new AsapStrategy();
        var scheduleResult = scheduler.Distribute(orderItems, employees, asap);

        // Определяем даты доставки
        DateOnly? minimalDate = null;
        DateOnly? maximalDate = null;

        if (scheduleResult.IsFeasible)
        {
            minimalDate = DateOnly.FromDateTime(scheduleResult.PlanEnd);
            maximalDate = DateOnly.FromDateTime(DateTime.Now).AddDays(30);
        }

        return new CartDto
        {
            CartItemsList = productCartItemsDto,
            MinimalDeliveryDate = minimalDate,
            MaximalDeliveryDate = maximalDate
        };
    }
}
