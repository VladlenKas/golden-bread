using GoldenBread.Application.Features.CompanyCart.Dtos;

namespace GoldenBread.Application.Features.CompanyCart.Queries.GetCart;

public sealed record class GetCartQuery(
    DateOnly? DesiredDeliveryDate) : IRequest<CartDto>;
