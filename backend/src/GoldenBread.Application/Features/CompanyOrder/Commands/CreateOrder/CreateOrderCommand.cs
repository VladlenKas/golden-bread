using GoldenBread.Application.Features.CompanyCart.Dtos;
using GoldenBread.Application.Features.CompanyOrder.Dtos;

namespace GoldenBread.Application.Features.CompanyOrder.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    DateOnly DesiredDeliveryDate) : IRequest<CreateOrderResult>;