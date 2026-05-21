using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Application.Abstractions.Services;
using GoldenBread.Application.Features.Orders.Dtos;

namespace GoldenBread.Application.Features.Orders.Queries;

public sealed record GetOrderByIdQuery(int Id) : IRequest<OrderDetailResponse?>;

public sealed class GetOrderByIdQueryHandler(
    IUnitConversionService unitConversion,
    IGoldenBreadContext context)
    : IRequestHandler<GetOrderByIdQuery, OrderDetailResponse?>
{
    public async Task<OrderDetailResponse?> Handle(
        GetOrderByIdQuery query,
        CancellationToken ct)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.Company)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Batch)
                    .ThenInclude(b => b.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.EmployeeTasks)
                    .ThenInclude(t => t.Employee)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.IngredientReservations)
                    .ThenInclude(r => r.IngredientBatch)
                        .ThenInclude(ib => ib.SupplierIngredient)
                            .ThenInclude(si => si.Ingredient)
            .FirstOrDefaultAsync(o => o.OrderId == query.Id, ct);

        if (order is null) return null;

        var items = order.OrderItems.Select(oi => new OrderItemDetail(
            oi.Batch.Product.Name,
            $"{oi.UnitsPerBatch}",
            oi.Quantity,
            oi.TotalAmount,
            oi.Status)).ToList();

        var tasks = order.OrderItems
            .SelectMany(oi => oi.EmployeeTasks)
            .Select(t => new EmployeeTaskDetail(
                t.EmployeeTaskId,
                t.Employee.Fullname,
                t.Status,
                t.StartTime,
                t.EndTime)).ToList();

        var reservations = order.OrderItems
            .SelectMany(oi => oi.IngredientReservations)
            .Select(r => new IngredientReservationDetail(
                r.IngredientBatch.SupplierIngredient.Ingredient.Name,
                unitConversion.ToBaseUnit(r.ReservedQuantity, r.IngredientBatch.SupplierIngredient.Unit),
                r.IngredientBatch.SupplierIngredient.Unit,
                r.IngredientBatch.DeliveryDate,
                r.IngredientBatch.ExpiryDate)).ToList();

        return new OrderDetailResponse(
            order.OrderId,
            order.Company?.Name ?? "-",
            order.Status,
            order.StartDate,
            order.EndDate,
            order.OrderItems.Sum(oi => oi.TotalAmount),
            order.CreatedAt,
            items,
            tasks,
            reservations);
    }
}