using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Domain.Entities;

namespace GoldenBread.Application.Features.CompanyCart.Commands.UpdateCartItem;

public sealed class UpdateCartItemCommandHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext) : 
    IRequestHandler<UpdateCartItemCommand, CartSummary>
{
    public async Task<CartSummary> Handle(
        UpdateCartItemCommand command, 
        CancellationToken ct)
    {
        var account = await accountContext.GetAccountAsync(ct);
        var company = account.GetCompany();

        var cartItem = await context.CartItems
            .AsTracking()
            .Include(ci => ci.Batch)  
                .ThenInclude(ci => ci.Product)  
            .FirstOrDefaultAsync(ci =>
                ci.CompanyId == company.CompanyId &&
                ci.Batch.ProductId == command.ProductId, 
                ct);

        var productBatch = await context.ProductBatches
            .FirstAsync(pb => 
                pb.ProductBatchId == command.ProductBatchId, 
                ct);

        if (command.Quantity <= 0 && cartItem != null)
        {
            context.CartItems.Remove(cartItem);
            await context.SaveChangesAsync(ct);
            return new CartSummary(0, 0, 0);
        }
        else if (cartItem == null)
        {
            await context.CartItems.AddAsync(
                CartItem.Create(
                    company,
                    productBatch, 
                    command.Quantity),
                ct);
        }
        else
        {
            cartItem.Update(
                productBatch, 
                command.Quantity);
        }

        await context.SaveChangesAsync(ct);

        var upd = await context.CartItems
            .Include(ci => ci.Batch)
               .ThenInclude(ci => ci.Product)
            .FirstAsync(ci =>
                ci.CompanyId == company.CompanyId &&
                ci.Batch.ProductId == command.ProductId,
                ct);

        return new CartSummary(
            upd.Quantity,
            upd.TotalCost,
            upd.BatchId);
    }
}
