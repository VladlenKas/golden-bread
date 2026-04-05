using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.CompanyCart.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Application.Features.CompanyCart.Queries.GetCart;

public class GetCartQueryHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext,
    IMapper mapper) :
    IRequestHandler<GetCartQuery, CartDto>
{
    public async Task<CartDto> Handle(
        GetCartQuery request,
        CancellationToken ct)
    {
        int companyId = await accountContext.GetRequiredCompanyIdAsync(ct);

        // Загружаем позиции корзины для DTO
        var productCartItemsDto = await context.CartItems
            .AsNoTracking()
            .Include(ci => ci.Batch)
                .ThenInclude(b => b.Product)
            .Where(ci => ci.CompanyId == companyId)
            .Select(ci => new ProductCartItemDto
            {
                ProductId = ci.Batch.ProductId,
                Name = ci.Batch.Product.Name,
                ProductionTimeMinutes = ci.Batch.Product.ProductionTimeMinutes,
                ProductBatchId = ci.Batch.ProductBatchId,
                QuantityPerBatch = ci.Batch.QuantityUnits,
                ImageUrl = ci.Batch.Product.ProductImages
                    .Select(pi => pi.ImagePath)
                    .FirstOrDefault(),
                IsFavorite = ci.Batch.Product.Favorites
                    .Any(f => f.CompanyId == companyId),
                QuantityInCart = ci.Quantity,
                TotalCostInCart = ci.TotalCost
            })
            .ToListAsync(ct);

        // Определяем даты доставки
        var minimalDate = request.DesiredDeliveryDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var selectedDate = request.DesiredDeliveryDate ?? minimalDate;

        return new CartDto
        {
            CartItemsList = mapper.Map<List<ProductCartItemDto>>(productCartItemsDto),
            MinimalDeliveryDate = minimalDate,
            SelectedDeliveryDate = selectedDate
        };
    }
}
