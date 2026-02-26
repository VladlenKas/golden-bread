using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.ProductCatalog.Commands.UpdateCartItem;

public sealed class UpdateCartItemCommandHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext) : 
    IRequestHandler<UpdateCartItemCommand, int>
{
    public async Task<int> Handle(
        UpdateCartItemCommand command, 
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);
        int companyId = account.Company.CompanyId;

        var productBatchInCart = await context.CartItems
            .FirstOrDefaultAsync(ci =>
                ci.CompanyId == companyId &&
                ci.BatchId == command.ProductBatchId,
                cancellationToken);

        if (command.Quantity <= 0 && productBatchInCart != null)
        {
            context.CartItems.Remove(productBatchInCart);
        }
        else if (command.Quantity > 0)
        {
            if (productBatchInCart == null)
                await context.CartItems.AddAsync(CartItem.Create(
                    companyId, 
                    command.ProductBatchId, 
                    command.Quantity),
                    cancellationToken);
            else
                productBatchInCart.Quantity = command.Quantity;
        }

        return command.Quantity <= 0 ? 0 : command.Quantity;
    }
}
