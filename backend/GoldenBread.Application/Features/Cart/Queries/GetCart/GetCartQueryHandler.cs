using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.Cart.Dtos;
using GoldenBread.Application.Services;

namespace GoldenBread.Application.Features.Cart.Queries.GetCart;

public class GetCartQueryHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext,
    IMapper mapper) :
    IRequestHandler<GetCartQuery, CartDto>
{
    public async Task<CartDto> Handle(
        GetCartQuery request, 
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);
        int companyId = account.Company.CompanyId;

        var cartItems = context.CartItems
            .Where(ci => ci.CompanyId == companyId)
            .Include(ci => ci.Batch)
                .ThenInclude(b => b.Product)
                    .ThenInclude(p => p.Favourites)
            .ToListAsync(cancellationToken);

        var tariffs = context.OrderTariffs
            .ToListAsync(cancellationToken);

        var productCartItems = mapper.Map<List<ProductCartItemDto>>(cartItems,
            opts => opts.Items["CompanyId"] = companyId);

        var tariffsList = mapper.Map<TariffDto>(tariffs);


        throw new Exception();
    }
}
