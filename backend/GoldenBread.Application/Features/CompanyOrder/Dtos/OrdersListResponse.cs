using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.CompanyOrder.Dtos;

public record OrdersListResponse(
    List<OrderListItemResponse> ListOrderItems);

public record OrderListItemResponse(
    int OrderId,
    DateOnly? StartDate,
    DateOnly EndDate,
    DateTime CreatedAt,
    OrderStatus Status,
    int QuantityOrderItems,
    decimal TotalAmount);
