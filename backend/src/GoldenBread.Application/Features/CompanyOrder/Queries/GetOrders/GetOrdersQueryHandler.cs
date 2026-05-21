using GoldenBread.Application.Abstractions.Data.Repositories;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.CompanyOrder.Dtos;

namespace GoldenBread.Application.Features.CompanyOrder.Queries.GetOrders;

public sealed class GetOrdersQueryHandler(
    ICurrentAccountContext accountContext,
    IOrderRepository orderRepository) 
    : IRequestHandler<GetOrdersQuery, OrdersListResponse>
{
    public async Task<OrdersListResponse> Handle(
        GetOrdersQuery query, 
        CancellationToken ct)
    {
        var companyId = await accountContext.GetCompanyIdAsync(ct);
        var orders = await orderRepository.GetAllByCompanyIdAsync(companyId!.Value, ct);

        var orderItems = orders.Select(order => new OrderListItemResponse(
            order.OrderId,
            order.StartDate.HasValue ? order.StartDate.Value : null,
            order.EndDate,
            order.CreatedAt,
            order.Status,
            order.OrderItems.Count,
            (int)order.OrderItems.Sum(oi => oi.TotalAmount))).ToList();

        return new OrdersListResponse(orderItems);
    }
}