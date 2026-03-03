using GoldenBread.Application.Features.Cart.Dtos;

namespace GoldenBread.Application.Features.Cart.Queries.GetCart;

public sealed record class GetCartQuery(
    DateOnly? DesiredDeliveryDate,
    int TariffId = 1) : IRequest<CartDto>;
