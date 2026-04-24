using GoldenBread.Application.Features.CompanyOrder.Dtos;

namespace GoldenBread.Application.Features.CompanyOrder.Queries.GetOrders;

public sealed record class GetOrdersQuery() : IRequest<OrdersListResponse>;
