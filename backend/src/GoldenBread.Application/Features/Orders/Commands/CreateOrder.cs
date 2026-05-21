using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Features.Orders.Dtos;
using GoldenBread.Domain.Entities;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Orders.Commands;

public sealed record CreateOrderCommand(CreateOrderRequest Request) : IRequest<int>;

public sealed class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrderItemRepository orderItemRepository,
    IGoldenBreadContext context,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var req = command.Request;

            var order = Order.Create(req.CompanyId, OrderStatus.Created, req.EndDate);

            await orderRepository.CreateAsync(order, ct);
            await unitOfWork.SaveChangesAsync(ct); // получаем OrderId

            var orderItems = new List<OrderItem>();

            foreach (var draft in req.Items)
            {
                var batch = await context.ProductBatches
                    .Include(pb => pb.Product)
                    .AsNoTracking()
                    .FirstAsync(b => b.ProductBatchId == draft.ProductBatchId, ct);

                var item = OrderItem.Create(
                    order.OrderId,
                    draft.ProductBatchId,
                    draft.Quantity,
                    batch.QuantityUnits,
                    batch.UnitPrice);

                orderItems.Add(item);
            }

            await orderItemRepository.CreateRangeAsync(orderItems, ct);
            await unitOfWork.CommitAsync(ct);

            return order.OrderId;
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}