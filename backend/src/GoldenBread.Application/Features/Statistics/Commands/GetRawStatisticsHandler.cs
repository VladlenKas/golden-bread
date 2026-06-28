using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Features.Statistics.Dtos;
using GoldenBread.Domain.Enums;

namespace GoldenBread.Application.Features.Statistics.Commands;

public class GetRawStatisticsHandler(IGoldenBreadContext context) : IRequestHandler<GetRawStatisticsQuery, RawStatisticsData>
{
    public async Task<RawStatisticsData> Handle(
        GetRawStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        // Один запрос: все заказы с товарами
        var orders = await context.OrderItems
            .AsNoTracking()
            .Where(oi => oi.Order.Status != OrderStatus.Canceled)
            .Where(oi => request.DateFrom == null || oi.Order.CreatedAt >= request.DateFrom)
            .Where(oi => request.DateTo == null || oi.Order.CreatedAt <= request.DateTo)
            .Select(oi => new OrderRawDto
            {
                OrderId = oi.OrderId,
                CompanyId = oi.Order.CompanyId,
                CompanyName = oi.Order.Company.Name,
                CreatedAt = oi.Order.CreatedAt,
                Status = oi.Order.Status.ToString(),
                ProductId = oi.Batch.ProductId,
                ProductName = oi.Batch.Product.Name,
                CategoryId = oi.Batch.Product.CategoryId,
                CategoryName = oi.Batch.Product.Category.Name,
                CategoryColor = oi.Batch.Product.Category.Color,
                Quantity = oi.Quantity,
                UnitsPerBatch = oi.UnitsPerBatch,
                UnitPrice = oi.UnitPrice,
                MarkupPercent = oi.Batch.MarkupPercent,
                CostPrice = oi.Batch.Product.CostPrice,
                ProductionTimeMinutes = oi.Batch.Product.ProductionTimeMinutes
            })
            .ToListAsync(cancellationToken);

        // Второй запрос: справочник товаров
        var products = await context.Products
            .AsNoTracking()
            .Select(p => new ProductRawDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                CategoryColor = p.Category.Color,
                CostPrice = p.CostPrice,
                ProductionTimeMinutes = p.ProductionTimeMinutes,
                ShelfLifeDays = p.ShelfLifeDays
            })
            .ToListAsync(cancellationToken);

        // Третий запрос: задачи сотрудников
        var tasks = await context.EmployeeTasks
            .AsNoTracking()
            .Where(et => et.OrderItem.Order.Status != OrderStatus.Canceled)
            .Where(et => request.DateFrom == null || et.OrderItem.Order.CreatedAt >= request.DateFrom)
            .Where(et => request.DateTo == null || et.OrderItem.Order.CreatedAt <= request.DateTo)
            .Select(et => new EmployeeTaskRawDto
            {
                EmployeeTaskId = et.EmployeeTaskId,
                ProductId = et.OrderItem.Batch.ProductId,
                ProductName = et.OrderItem.Batch.Product.Name,
                AssignedQuantity = et.AssignedQuantity,
                CompletedQuantity = et.CompletedQuantity,
                StartTime = et.StartTime,
                EndTime = et.EndTime,
                Status = et.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        // Четвёртый запрос: избранное
        var favorites = await context.Favorites
            .AsNoTracking()
            .Select(f => new FavoriteRawDto
            {
                ProductId = f.ProductId,
                ProductName = f.Product.Name,
                CompanyId = f.CompanyId,
                CategoryId = f.Product.CategoryId
            })
            .ToListAsync(cancellationToken);

        return new RawStatisticsData
        {
            Orders = orders,
            Products = products,
            EmployeeTasks = tasks,
            Favorites = favorites
        };
    }
}