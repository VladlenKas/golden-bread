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
        var company = account.Company;

        var cartItem = await context.CartItems
            .AsTracking()
            .Include(ci => ci.Batch)  
                .ThenInclude(ci => ci.Product)  
            .FirstOrDefaultAsync(ci =>
                ci.CompanyId == company.CompanyId &&
                ci.Batch.ProductId == command.ProductId, 
                cancellationToken);

        var productBatch = await context.ProductBatches
            .FirstAsync(pb => 
                pb.ProductBatchId == command.ProductBatchId, 
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
                    company,
                    productBatch, 
                    command.Quantity),
                cancellationToken);
        }
        else
        {
            cartItem.Update(
                productBatch, 
                command.Quantity);
        }

        await context.SaveChangesAsync(cancellationToken);

        var upd = await context.CartItems
            .Include(ci => ci.Batch)
               .ThenInclude(ci => ci.Product)
            .FirstAsync(ci =>
                ci.CompanyId == company.CompanyId &&
                ci.Batch.ProductId == command.ProductId,
                cancellationToken);

        return new CartSummary(
            upd.Quantity,
            upd.TotalCost,
            upd.BatchId);
    }
}
