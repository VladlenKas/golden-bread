using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.ProductCatalog.Commands.UpdateCartItem;

public sealed class UpdateCartItemCommandHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext) : 
    IRequestHandler<UpdateCartItemCommand, CartSummary>
{
    public async Task<CartSummary> Handle(
        UpdateCartItemCommand command, 
        CancellationToken cancellationToken)
    {
        var account = await accountContext.GetAccountAsync(cancellationToken);
        int companyId = account.Company.CompanyId;

        var cartItem = await context.CartItems
            .AsTracking()
            .Include(ci => ci.Batch)  
                .ThenInclude(ci => ci.Product)  
            .FirstOrDefaultAsync(ci =>
                ci.CompanyId == companyId &&
                ci.Batch.ProductId == command.ProductId, 
                cancellationToken);

        if (command.Quantity <= 0 && cartItem != null)
        {
            context.CartItems.Remove(cartItem);
            return new CartSummary(0, 0, 0);
        }
        else if (cartItem == null)
        {
            await context.CartItems.AddAsync(
                CartItem.Create(
                    companyId, 
                    command.ProductBatchId, 
                    command.Quantity),
                cancellationToken);
        }
        else
        {
            cartItem.Update(
                command.ProductBatchId, 
                command.Quantity);
        }

        await context.SaveChangesAsync(cancellationToken);

        var upd = await context.CartItems
            .Include(ci => ci.Batch)
               .ThenInclude(ci => ci.Product)
            .FirstAsync(ci =>
                ci.CompanyId == companyId &&
                ci.Batch.ProductId == command.ProductId,
                cancellationToken);

        return new CartSummary(
            upd.Quantity,
            upd.TotalPrice,
            upd.BatchId);
    }
}
