using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.CompanyCart.Dtos;
using GoldenBread.Application.Features.CompanyCart.Services;

namespace GoldenBread.Application.Features.CompanyCart.Queries.GetCart;

public class GetCartQueryHandler(
    IGoldenBreadContext context,
    ICurrentAccountContext accountContext,
    IMapper mapper,
    IDeliveryDateCalculator deliveryCalculator) : 
    IRequestHandler<GetCartQuery, CartDto>
{
    public async Task<CartDto> Handle(
        GetCartQuery request,
        CancellationToken ct)
    {
        int companyId = await accountContext.GetCompanyIdAsync(ct);
        var now = DateTime.UtcNow;

        // Загружаем позиции корзины для CalculateMinimalDeliveryDate
        var productCartEntities = await context.CartItems
            .AsNoTracking()
            .Include(ci => ci.Batch)
                .ThenInclude(b => b.Product)
            .Where(ci => ci.CompanyId == companyId)
            .ToListAsync(ct);

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
                QuantityPerBatch = ci.Batch.QuantityPerBatch,    
                ImageUrl = ci.Batch.Product.ProductImages
                    .Select(pi => pi.ImagePath)
                    .FirstOrDefault(),
                IsFavorite = ci.Batch.Product.Favorites
                    .Any(f => f.CompanyId == companyId),
                QuantityInCart = ci.Quantity,
                TotalCostInCart = ci.TotalCost
            })
            .ToListAsync(ct);

        // Загружаем все тарифы для отображения
        var tariffs = await context.OrderTariffs
            .ToListAsync(ct);

        // Загружаем выбранный тариф
        var selectedTariff = await context.OrderTariffs
            .FirstOrDefaultAsync(t => t.OrderTariffId == request.TariffId, ct)
            ?? tariffs.FirstOrDefault()
            ?? throw new InvalidOperationException("No active tariff found");

        // Загружаем активных сотрудников с их задачами
        var activeEmployees = await context.Employees
            .Where(e => e.DeletedAt == null)
            .Include(e => e.EmployeeTasks.Where(et => et.EndTime > now))
            .ToListAsync(ct);

        // Рассчитываем минимальную дату доставки
        var minimalDeliveryDate = deliveryCalculator
            .CalculateMinimalDeliveryDate(
                productCartEntities, 
                selectedTariff, activeEmployees, now);

        // Определяем итоговую дату доставки
        var deliveryDate = request.DesiredDeliveryDate.HasValue
            && request.DesiredDeliveryDate.Value >= minimalDeliveryDate
                ? request.DesiredDeliveryDate.Value
                : minimalDeliveryDate;

        return new CartDto
        {
            CartItemsList = mapper.Map<List<ProductCartItemDto>>(productCartItemsDto),
            TariffItemsList = mapper.Map<List<TariffDto>>(tariffs),
            MinimalDeliveryDate = minimalDeliveryDate,
            SelectedDeliveryDate = deliveryDate,
            SelectedTariffId = selectedTariff.OrderTariffId
        };
    }
}
